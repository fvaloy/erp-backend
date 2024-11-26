using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Employees.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record DeleteEmployeeCommand(string Id) : IRequest<Result>;

public sealed class DeleteEmployeeCommandHandler(IAppDbContext context) : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeDb = await _context.Employees.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (employeeDb is null) return Result.NotFound($"{nameof(Employee)} was not found");

        employeeDb.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success($"{nameof(Employee)} successfully eliminated");
    }
}

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}