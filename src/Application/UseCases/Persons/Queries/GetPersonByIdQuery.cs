using Application.Auth;
using Application.Persistence;
using Application.UseCases.Persons.Commands;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Queries;

[Authorize(Roles = UserRoles.USER)]
public sealed record GetPersonByIdQuery(string Id) : IRequest<Result<PersonDto>>;

public sealed class GetPersonByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetPersonByIdQuery, Result<PersonDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PersonDto>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _context.Persons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (person is null) return Result.NotFound("the person was not found");
        return PersonDto.FromPerson(person);
    }
}

public sealed class GetPersonByIdQueryValidator : AbstractValidator<GetPersonByIdQuery>
{
    public GetPersonByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
    }
}