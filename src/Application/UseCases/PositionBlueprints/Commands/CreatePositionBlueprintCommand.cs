using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.PositionBlueprints.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record CreatePositionBlueprintCommand(string Name, decimal MaxSalary, decimal MinSalary) : IRequest<Result<PositionBlueprintDto>>
{
    public PositionBlueprint ToPositionBlueprint()
    {
        return new PositionBlueprint
        {
            Name = Name,
            MaxSalary = MaxSalary,
            MinSalary = MinSalary
        };
    }
}
public sealed record PositionBlueprintDto(string Id, string Name, decimal MaxSalary, decimal MinSalary)
{
    public static PositionBlueprintDto FromPositionBlueprint(PositionBlueprint bp)
        => new (bp.Id, bp.Name, bp.MaxSalary, bp.MinSalary);
}

public sealed class CreatePositionBlueprintCommandHandlerj(IAppDbContext context) : IRequestHandler<CreatePositionBlueprintCommand, Result<PositionBlueprintDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PositionBlueprintDto>> Handle(CreatePositionBlueprintCommand request, CancellationToken cancellationToken)
    {
        var blueprintExist = await _context.PositionBlueprints.AnyAsync(pb => pb.Name == request.Name, cancellationToken);
        if (blueprintExist) return Result.Error("There is already a PositionBlueprint who has that name");

        var positionBlueprint = request.ToPositionBlueprint();

        await _context.PositionBlueprints.AddAsync(positionBlueprint, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return PositionBlueprintDto.FromPositionBlueprint(positionBlueprint);
    }
}

public sealed class CreatePositionBlueprintCommandValidator : AbstractValidator<CreatePositionBlueprintCommand>
{
    public CreatePositionBlueprintCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.MaxSalary).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.MinSalary).NotEmpty().WithMessage("{PropertyName} is required");
    }
}