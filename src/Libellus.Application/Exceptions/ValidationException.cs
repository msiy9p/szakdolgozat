using FluentValidation.Results;

namespace Libellus.Application.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; init; }

    public ValidationException() : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(key => key.PropertyName, e => e.ErrorMessage)
            .ToDictionary(key => key.Key, messages => messages.ToArray());
    }
}