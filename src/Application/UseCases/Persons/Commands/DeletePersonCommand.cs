using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record DeletePersonCommand(string Id) : IRequest<Result>;

public sealed class DeletePersonCommandHandler(IAppDbContext context) : IRequestHandler<DeletePersonCommand, Result>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var personDb = await _context.Persons.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (personDb is null) return Result.NotFound("the person was not found");

        personDb.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("person successfully eliminated");
    }
}

public class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}