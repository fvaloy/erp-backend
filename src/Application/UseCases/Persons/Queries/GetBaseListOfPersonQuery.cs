using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetBaseListOfPersonQuery : IRequest<List<BasePersonDto>>;
public sealed record BasePersonDto(string Id, string FullName, string DNI)
{
    public static BasePersonDto FromPerson(Person p)
        => new (p.Id, p.FirstName + " " + p.LastName, p.DNI);
};

public sealed class GetBaseListOfPersonQueryHandler(IAppDbContext context) : IRequestHandler<GetBaseListOfPersonQuery, List<BasePersonDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<List<BasePersonDto>> Handle(GetBaseListOfPersonQuery request, CancellationToken cancellationToken)
    {
        var persons = await _context.Persons
            .Where(p => p.IsDeleted == false)
            .Select(p => BasePersonDto.FromPerson(p))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return persons;
    }
}