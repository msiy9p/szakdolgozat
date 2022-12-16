using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class WordCountInvalidException : ArgumentException
{
    public WordCountInvalidException()
    {
    }

    public WordCountInvalidException(string? message) : base(message)
    {
    }

    public WordCountInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public WordCountInvalidException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public WordCountInvalidException(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }

    protected WordCountInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}