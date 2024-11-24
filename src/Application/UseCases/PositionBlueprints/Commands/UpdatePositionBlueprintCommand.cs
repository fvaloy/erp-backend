using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.PositionBlueprints.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record UpdatePositionBlueprintCommand(string Id, string Name, decimal MaxSalary, decimal MinSalary) : IRequest<Result<PositionBlueprintDto>>
{
    public PositionBlueprint UpdatePositionBlueprint(PositionBlueprint positionBlueprint)
    {
        positionBlueprint.Name = Name;
        positionBlueprint.MaxSalary = MaxSalary;
        positionBlueprint.MinSalary = MinSalary;
        return positionBlueprint;
    }
}

public sealed class UpdatePositionBlueprintCommandHandlerj(IAppDbContext context) : IRequestHandler<UpdatePositionBlueprintCommand, Result<PositionBlueprintDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PositionBlueprintDto>> Handle(UpdatePositionBlueprintCommand request, CancellationToken cancellationToken)
    {
        
        var blueprintDb = await _context.PositionBlueprints.FirstOrDefaultAsync(pb => pb.Id == request.Id, cancellationToken);
        if (blueprintDb is null) return Result.NotFound("PositionBlueprint was not found");

        if (blueprintDb.Name != request.Name)
        {
            var blueprintExist = await _context.PositionBlueprints.AnyAsync(pb => pb.Name == request.Name, cancellationToken);
            if (blueprintExist) return Result.Error("There is already a PositionBlueprint who has that Name");
        }

        var positionBlueprint = request.UpdatePositionBlueprint(blueprintDb);
        await _context.SaveChangesAsync(cancellationToken);

        return PositionBlueprintDto.FromPositionBlueprint(positionBlueprint);
    }
}

public sealed class UpdatePositionBlueprintCommandValidator : AbstractValidator<UpdatePositionBlueprintCommand>
{
    public UpdatePositionBlueprintCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.MaxSalary).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.MinSalary).NotEmpty().WithMessage("{PropertyName} is required");
    }
}