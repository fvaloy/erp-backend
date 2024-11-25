using Application.Auth;
using Application.Pagination;
using Application.Persistence;
using Application.UseCases.Persons.Commands;
using Application.UseCases.PositionBlueprints.Commands;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetPaginatedPositionBlueprintsQuery(int PageNumber, int PageSize, string Search) : IRequest<PaginatedList<PositionBlueprintDto>>;

public sealed class GetPaginatedPositionBlueprintsQueryHandler(IAppDbContext context) : IRequestHandler<GetPaginatedPositionBlueprintsQuery, PaginatedList<PositionBlueprintDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<PaginatedList<PositionBlueprintDto>> Handle(GetPaginatedPositionBlueprintsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PositionBlueprints.Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(p => p.Name.Contains(request.Search, StringComparison.CurrentCultureIgnoreCase));
        }

        var count = await query.CountAsync(cancellationToken);

        var persons = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => PositionBlueprintDto.FromPositionBlueprint(p))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new PaginatedList<PositionBlueprintDto>(persons, count, request.PageNumber, request.PageSize);
    }
}

public sealed class GetPaginatedPositionBlueprintsQueryValidator : AbstractValidator<GetPaginatedPositionBlueprintsQuery>
{
    public GetPaginatedPositionBlueprintsQueryValidator()
    {
        RuleFor(x => x.PageNumber).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.PageSize).NotEmpty().WithMessage("{PropertyName} is required");
    }
}