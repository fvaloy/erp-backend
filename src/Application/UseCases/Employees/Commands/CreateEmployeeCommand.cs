using Application.Auth;
using Application.Persistence;
using Application.UseCases.Persons.Commands;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Employees.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record CreateEmployeeCommand(
    string PersonId,
    string JobPositionId,
    decimal Salary) : IRequest<Result<EmployeeDto>>
{
    public Employee ToEmployee()
    {
        return new Employee
        {
            PersonId = PersonId,
            JobPositionId = JobPositionId,
            Salary = Salary
        };
    }
}

public sealed record EmployeeDto(
    string Id,
    string FullName,
    string PersonId,
    string JobPosition,
    string JobPositionId,
    decimal Salary)
{
    public static EmployeeDto FromEmployee(Employee e)
        => new(e.Id, $"{e.Person.FirstName} {e.Person.LastName}", e.PersonId, e.JobPosition.PositionBlueprint.Name, e.JobPositionId, e.Salary);
}

public sealed class CreateEmployeeCommandHandler(IAppDbContext context) : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);
        if (person is null) return Result.Error($"{nameof(Person)} was not found");
        var jobPosition = await _context.JobPositions.Include(jp => jp.PositionBlueprint).FirstOrDefaultAsync(p => p.Id == request.JobPositionId, cancellationToken);
        if (jobPosition is null) return Result.Error($"{nameof(JobPosition)} was not found");

        if (request.Salary < jobPosition.PositionBlueprint.MinSalary || request.Salary > jobPosition.PositionBlueprint.MaxSalary)
        {
            return Result.Error("The salary is not in the range specified in the job template");
        }

        var employee = request.ToEmployee();
        employee.JobPosition = jobPosition;
        employee.Person = person;

        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return EmployeeDto.FromEmployee(employee);
    }
}

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.JobPositionId).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.Salary).NotEmpty().WithMessage("{PropertyName} is required");
    }
}