using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class ScoreMultiplierInvalidException : ArgumentException
{
    public ScoreMultiplierInvalidException()
    {
    }

    public ScoreMultiplierInvalidException(string? message) : base(message)
    {
    }

    public ScoreMultiplierInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ScoreMultiplierInvalidException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public ScoreMultiplierInvalidException(string? message, string? paramName, Exception? innerException) : base(
        message, paramName, innerException)
    {
    }

    protected ScoreMultiplierInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}