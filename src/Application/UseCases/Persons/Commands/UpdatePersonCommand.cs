using Application.Auth;
using Application.Exceptions;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record UpdatePersonCommand(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string DNI,
    DateTime DateOfBirth) : IRequest<Result<PersonDto>>
{
    public Person ToPerson()
    {
        return new Person
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            DNI = DNI,
            DateOfBirth = DateOfBirth
        };
    }

    public Person UpdatePerson(Person person)
    {
        person.FirstName = FirstName;
        person.LastName = LastName;
        person.Email = Email;
        person.PhoneNumber = PhoneNumber;
        person.DNI = DNI;
        person.DateOfBirth = DateOfBirth;
        return person;
    }
}

public sealed class UpdatePersonCommandHandler(IAppDbContext context) : IRequestHandler<UpdatePersonCommand, Result<PersonDto>>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<PersonDto>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var personDb = await _context.Persons.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (personDb is null) return Result.NotFound("Person was not found");

        var person = request.UpdatePerson(personDb);

        await _context.SaveChangesAsync(cancellationToken);

        return PersonDto.FromPerson(person);
    }
}

public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.DNI).NotEmpty().WithMessage("{PropertyName} is required");
    }
}