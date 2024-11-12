using FluentValidation;
using MediatR;

namespace Application.UseCases.Greeting.Queries;

public record GreetingQuery(string Name) : IRequest<string>;

public class GreetingQueryHandler : IRequestHandler<GreetingQuery, string>
{
    public Task<string> Handle(GreetingQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Hello {request.Name}!");
    }
}

public class GreetingQueryValidator : AbstractValidator<GreetingQuery>
{
    public GreetingQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} is required");
    }
}