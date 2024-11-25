using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.JobPositions.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record DeleteJobPositionCommand(string Id) : IRequest<Result>;

public sealed class DeleteJobPositionCommandHandler(IAppDbContext context) : IRequestHandler<DeleteJobPositionCommand, Result>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result> Handle(DeleteJobPositionCommand request, CancellationToken cancellationToken)
    {
        var jp = await _context.JobPositions.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (jp is null) return Result.NotFound($"{nameof(JobPosition)} was not found");

        jp.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success($"{nameof(JobPosition)} successfully eliminated");
    }
}

public class DeleteJobPositionCommandValidator : AbstractValidator<DeleteJobPositionCommand>
{
    public DeleteJobPositionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}