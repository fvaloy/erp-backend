using Application.Auth;
using Application.Pagination;
using Application.Persistence;
using Application.UseCases.Persons.Commands;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetPaginatedPersonsQuery(int PageNumber, int PageSize, string Search) : IRequest<PaginatedList<PersonDto>>;

public sealed class GetPaginatedPersonsQueryHandler(IAppDbContext context) : IRequestHandler<GetPaginatedPersonsQuery, PaginatedList<PersonDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<PaginatedList<PersonDto>> Handle(GetPaginatedPersonsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Persons.Where(p => p.IsDeleted == false);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(p => (p.FirstName + p.LastName + p.DNI + p.PhoneNumber + p.Email).ToUpper().Contains(request.Search.ToUpper()));
        }

        var count = await query.CountAsync(cancellationToken);

        var persons = await query
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => PersonDto.FromPerson(p))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new PaginatedList<PersonDto>(persons, count, request.PageNumber, request.PageSize);
    }
}

public sealed class GetPaginatedPersonsQueryValidator : AbstractValidator<GetPaginatedPersonsQuery>
{
    public GetPaginatedPersonsQueryValidator()
    {
        RuleFor(x => x.PageNumber).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.PageSize).NotEmpty().WithMessage("{PropertyName} is required");
    }
}