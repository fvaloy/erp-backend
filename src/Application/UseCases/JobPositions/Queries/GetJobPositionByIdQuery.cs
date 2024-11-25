using Application.Auth;
using Application.Persistence;
using Application.UseCases.JobPositions.Commands;
using Application.UseCases.Persons.Commands;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.JobPositions.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetJobPositionByIdQuery(string Id) : IRequest<Result<JobPositionDto>>;

public sealed class GetJobPositionByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetJobPositionByIdQuery, Result<JobPositionDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<JobPositionDto>> Handle(GetJobPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var jp = await _context.JobPositions
            .Include(jp => jp.PositionBlueprint)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (jp is null) return Result.NotFound($"{nameof(JobPosition)} was not found");
        return JobPositionDto.FromJobPosition(jp);
    }
}

public sealed class GetJobPositionByIdQueryValidator : AbstractValidator<GetJobPositionByIdQuery>
{
    public GetJobPositionByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}