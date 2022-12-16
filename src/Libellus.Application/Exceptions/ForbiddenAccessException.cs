using System.Runtime.Serialization;

namespace Libellus.Application.Exceptions;

public class ForbiddenAccessException : Exception
{
    private readonly List<string?> _errors = new();

    public IReadOnlyCollection<string?> Errors => _errors;

    public ForbiddenAccessException()
    {
    }

    protected ForbiddenAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ForbiddenAccessException(string? message) : base(message)
    {
        _errors.Add(message);
    }

    public ForbiddenAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
        _errors.Add(message);
    }

    public ForbiddenAccessException(IEnumerable<string?> messages) : base(messages.FirstOrDefault())
    {
        _errors.AddRange(messages);
    }

    public ForbiddenAccessException(IEnumerable<string?> messages, Exception? innerException) : base(
        messages.FirstOrDefault(), innerException)
    {
        _errors.AddRange(messages);
    }
}