using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.PositionBlueprints.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record DeletePositionBlueprintCommand(string Id) : IRequest<Result>;

public sealed class DeletePositionBlueprintCommandHandler(IAppDbContext context) : IRequestHandler<DeletePositionBlueprintCommand, Result>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result> Handle(DeletePositionBlueprintCommand request, CancellationToken cancellationToken)
    {
        var pbDb = await _context.PositionBlueprints.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (pbDb is null) return Result.NotFound($"the {nameof(PositionBlueprint)} was not found");

        pbDb.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success($"{nameof(PositionBlueprint)} successfully eliminated");
    }
}

public class DeletePositionBlueprintCommandValidator : AbstractValidator<DeletePositionBlueprintCommand>
{
    public DeletePositionBlueprintCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}