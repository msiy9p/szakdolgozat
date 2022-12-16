using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class PartialDateInvalidException : ArgumentException
{
    public PartialDateInvalidException()
    {
    }

    public PartialDateInvalidException(string? message) : base(message)
    {
    }

    public PartialDateInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public PartialDateInvalidException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public PartialDateInvalidException(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }

    protected PartialDateInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}