using Application.Auth;
using Application.Exceptions;
using Application.Persistence;
using Application.UseCases.Persons.Commands;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Employees.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record UpdateEmployeeCommand(
    string Id,
    string JobPositionId,
    decimal Salary) : IRequest<Result<EmployeeDto>>
{
    public Employee UpdateEmployee(Employee employee)
    {
        employee.Salary = Salary;
        employee.JobPositionId = JobPositionId;
        return employee;
    }
}

public sealed class UpdateEmployeeCommandHandler(IAppDbContext context) : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<EmployeeDto>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeDb = await _context.Employees
            .Include(e => e.Person)
            .Include(e => e.JobPosition)
                .ThenInclude(jp => jp.PositionBlueprint)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (employeeDb is null) return Result.NotFound($"{nameof(Employee)} was not found");

        if (employeeDb.JobPositionId != request.JobPositionId)
        {
            var jobPosition = await _context.JobPositions.FirstOrDefaultAsync(jp => jp.Id == request.JobPositionId, cancellationToken);
            if (jobPosition is null) return Result.Error($"{nameof(JobPosition)} was not found");
            employeeDb.JobPosition = jobPosition;
        }

        var employee = request.UpdateEmployee(employeeDb);

        await _context.SaveChangesAsync(cancellationToken);

        return EmployeeDto.FromEmployee(employee);
    }
}

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.JobPositionId).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.Salary).NotEmpty().WithMessage("{PropertyName} is required");
    }
}