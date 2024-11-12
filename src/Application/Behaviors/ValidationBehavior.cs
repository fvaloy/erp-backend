using FluentValidation;
using MediatR;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> (IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults
                .Where(v => v.Errors.Count != 0)
                .SelectMany(v => v.Errors)
                .ToList();
            if (failures.Any())
                throw new ValidationException(failures);
        }
        return await next();
    }
}