using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class NumberInSeriesInvalidException : ArgumentException
{
    public NumberInSeriesInvalidException()
    {
    }

    public NumberInSeriesInvalidException(string? message) : base(message)
    {
    }

    public NumberInSeriesInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public NumberInSeriesInvalidException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public NumberInSeriesInvalidException(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }

    protected NumberInSeriesInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}