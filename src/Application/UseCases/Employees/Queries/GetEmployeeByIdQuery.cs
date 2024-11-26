using Application.Auth;
using Application.Persistence;
using Application.UseCases.Employees.Commands;
using Application.UseCases.Persons.Commands;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Employees.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetEmployeeByIdQuery(string Id) : IRequest<Result<EmployeeDto>>;

public sealed class GetEmployeeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Person)
            .Include(e => e.JobPosition)
                .ThenInclude(jp => jp.PositionBlueprint)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (employee is null) return Result.NotFound($"{nameof(Employee)} was not found");
        return EmployeeDto.FromEmployee(employee);
    }
}

public sealed class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}