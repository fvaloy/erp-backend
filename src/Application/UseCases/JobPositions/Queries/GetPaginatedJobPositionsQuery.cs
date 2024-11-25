using Application.Auth;
using Application.Pagination;
using Application.Persistence;
using Application.UseCases.JobPositions.Commands;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.JobPositions.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetPaginatedJobPositionsQuery(int PageNumber, int PageSize, string Search, string PositionBlueprintId, bool? IsVacant) : IRequest<PaginatedList<JobPositionDto>>;

public sealed class GetPaginatedJobPositionsQueryHandler(IAppDbContext context) : IRequestHandler<GetPaginatedJobPositionsQuery, PaginatedList<JobPositionDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<PaginatedList<JobPositionDto>> Handle(GetPaginatedJobPositionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.JobPositions.Include(jp => jp.PositionBlueprint).Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(request.PositionBlueprintId))
        {
            query = query.Where(jp => jp.PositionBlueprintId == request.PositionBlueprintId);
        }
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(p => p.PositionBlueprint.Name.Contains(request.Search, StringComparison.CurrentCultureIgnoreCase));
        }
        if (request.IsVacant is not null)
        {
            query = query.Where(p => p.IsVancant == request.IsVacant);
        }

        var count = await query.CountAsync(cancellationToken);

        var jps = await query
            .OrderBy(jp => jp.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(jp => JobPositionDto.FromJobPosition(jp))
            .ToListAsync(cancellationToken);

        return new PaginatedList<JobPositionDto>(jps, count, request.PageNumber, request.PageSize);
    }
}

public sealed class GetPaginatedJobPositionsQueryValidator : AbstractValidator<GetPaginatedJobPositionsQuery>
{
    public GetPaginatedJobPositionsQueryValidator()
    {
        RuleFor(x => x.PageNumber).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.PageSize).NotEmpty().WithMessage("{PropertyName} is required");
    }
}