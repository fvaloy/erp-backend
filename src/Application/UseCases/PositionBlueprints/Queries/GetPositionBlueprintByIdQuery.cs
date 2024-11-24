using Application.Auth;
using Application.Persistence;
using Application.UseCases.PositionBlueprints.Commands;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetPositionBluentprintByIdQuery(string Id) : IRequest<Result<PositionBlueprintDto>>;

public sealed class GetPositionBluentprintByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetPositionBluentprintByIdQuery, Result<PositionBlueprintDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PositionBlueprintDto>> Handle(GetPositionBluentprintByIdQuery request, CancellationToken cancellationToken)
    {
        var pb = await _context.PositionBlueprints.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (pb is null) return Result.NotFound($"the {nameof(PositionBlueprint)}  was not found");
        return PositionBlueprintDto.FromPositionBlueprint(pb);
    }
}

public sealed class GetPositionBluentprintByIdQueryValidator : AbstractValidator<GetPositionBluentprintByIdQuery>
{
    public GetPositionBluentprintByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}