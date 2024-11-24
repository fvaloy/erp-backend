using Application.Auth;
using Application.Persistence;
using Domain.Entities;
using FluentValidation;
using Francisvac.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.Commands;

[Authorize(Roles = UserRoles.ADMIN)]
public sealed record CreatePersonCommand(
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
}

public sealed record PersonDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string DNI,
    DateTime DateOfBirth)
{
    public static PersonDto FromPerson(Person p)
        => new (p.Id, p.FirstName, p.LastName, p.Email, p.PhoneNumber, p.DNI, p.DateOfBirth);
}

public sealed class CreatePersonCommandHandler(IAppDbContext context) : IRequestHandler<CreatePersonCommand, Result<PersonDto>>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<PersonDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var personExist = await _context.Persons.AnyAsync(p => p.DNI == request.DNI, cancellationToken);
        if (personExist) return Result.Error("There is already a person who has that DNI");

        var person = request.ToPerson();

        await _context.Persons.AddAsync(person, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return PersonDto.FromPerson(person);
    }
}

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(x => x.DNI).NotEmpty().WithMessage("{PropertyName} is required");
    }
}