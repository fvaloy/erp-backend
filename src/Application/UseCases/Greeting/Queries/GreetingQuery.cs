using Application.Auth;
using FluentValidation;
using Francisvac.Result;
using MediatR;

namespace Application.UseCases.Greeting.Queries;

[Authorize(Roles = "other-role")]
public record GreetingQuery(string Name) : IRequest<Result<string>>;

public class GreetingQueryHandler : IRequestHandler<GreetingQuery, Result<string>>
{
    public Task<Result<string>> Handle(GreetingQuery request, CancellationToken cancellationToken)
    {
        if (request.Name == "eloyka")
        {
            return Task.FromResult(Result<string>.Error("hell no >:("));
        }

        return Task.FromResult(Result<string>.Success($"Hello {request.Name}!"));
    }
}

public class GreetingQueryValidator : AbstractValidator<GreetingQuery>
{
    public GreetingQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required");
    }
}