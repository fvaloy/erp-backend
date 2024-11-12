using FluentValidation.Results;

namespace Application.Exceptions;

public class ValidationException(IEnumerable<ValidationFailure> failures) : Exception
{
    public IDictionary<string, string[]> Errors { get; } = failures
        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
        .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
}
