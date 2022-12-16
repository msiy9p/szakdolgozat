using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class IsbnInvalidException : ArgumentException
{
    public IsbnInvalidException()
    {
    }

    public IsbnInvalidException(string? message) : base(message)
    {
    }

    public IsbnInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public IsbnInvalidException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public IsbnInvalidException(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }

    protected IsbnInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}