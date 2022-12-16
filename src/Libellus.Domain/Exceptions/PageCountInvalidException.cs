using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class PageCountInvalidException : ArgumentException
{
    public PageCountInvalidException()
    {
    }

    public PageCountInvalidException(string? message) : base(message)
    {
    }

    public PageCountInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public PageCountInvalidException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public PageCountInvalidException(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }

    protected PageCountInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}