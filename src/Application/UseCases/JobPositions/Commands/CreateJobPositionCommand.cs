using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.JobPositions.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record CreateJobPositionCommand(string PositionBlueprintId, int Amount) : IRequest<Result<List<JobPositionDto>>>;

public sealed record JobPositionDto(
    string Id,
    string PositionBlueprintId,
    string PositionName,
    bool IsVancat)
{
    public static JobPositionDto FromJobPosition(JobPosition jp)
        => new(jp.Id, jp.PositionBlueprintId, jp.PositionBlueprint.Name, jp.IsVancant);
}

public sealed class CreateJobPositionCommandHandler(IAppDbContext context) : IRequestHandler<CreateJobPositionCommand, Result<List<JobPositionDto>>>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<List<JobPositionDto>>> Handle(CreateJobPositionCommand request, CancellationToken cancellationToken)
    {
        var positionBlueprint = await _context.PositionBlueprints.FirstOrDefaultAsync(pb => pb.Id == request.PositionBlueprintId, cancellationToken);
        if (positionBlueprint is null) return Result.NotFound($"{nameof(PositionBlueprint)} was not found");
        
        List<JobPosition> jobPositions = [];
        for (int i = 0; i < request.Amount; i++)
        {
            jobPositions.Add(new JobPosition()
            {
                PositionBlueprintId = request.PositionBlueprintId,
                PositionBlueprint = positionBlueprint
            });
        }

        await _context.JobPositions.AddRangeAsync(jobPositions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return jobPositions.Select(JobPositionDto.FromJobPosition).ToList();
    }
}

public class CreateJobPositionCommandValidator : AbstractValidator<CreateJobPositionCommand>
{
    public CreateJobPositionCommandValidator()
    {
        RuleFor(x => x.PositionBlueprintId).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.Amount).NotEmpty().WithMessage("{PropertyName} is required");
    }
}