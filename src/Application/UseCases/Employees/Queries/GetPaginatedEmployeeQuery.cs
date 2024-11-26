using Application.Auth;
using Application.Pagination;
using Application.Persistence;
using Application.UseCases.Employees.Commands;
using Application.UseCases.Persons.Commands;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetPaginatedEmployeeQuery(int PageNumber, int PageSize, string Search, string PositionBlueprintId) : IRequest<PaginatedList<EmployeeDto>>;

public sealed class GetPaginatedEmployeeQueryHandler(IAppDbContext context) : IRequestHandler<GetPaginatedEmployeeQuery, PaginatedList<EmployeeDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<PaginatedList<EmployeeDto>> Handle(GetPaginatedEmployeeQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Include(e => e.Person)
            .Include(e => e.JobPosition)
                .ThenInclude(jp => jp.PositionBlueprint)
            .Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(e => (e.Id + e.Person.FirstName + e.Person.LastName + e.JobPosition.PositionBlueprint.Name).ToUpper().Contains(request.Search.ToUpper()));
        }

        if (!string.IsNullOrEmpty(request.PositionBlueprintId))
        {
            query = query.Where(e => e.JobPosition.PositionBlueprintId == request.PositionBlueprintId);
        }

        var count = await query.CountAsync(cancellationToken);

        var employees = await query
            .OrderBy(e => e.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => EmployeeDto.FromEmployee(e))
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new PaginatedList<EmployeeDto>(employees, count, request.PageNumber, request.PageSize);
    }
}

public sealed class GetPaginatedEmployeeQueryValidator : AbstractValidator<GetPaginatedEmployeeQuery>
{
    public GetPaginatedEmployeeQueryValidator()
    {
        RuleFor(x => x.PageNumber).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.PageSize).NotEmpty().WithMessage("{PropertyName} is required");
    }
}