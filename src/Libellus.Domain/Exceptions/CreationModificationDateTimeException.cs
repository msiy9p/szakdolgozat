using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class CreationModificationDateTimeException : ArgumentException
{
    public CreationModificationDateTimeException() : this("Creation date is bigger then modification date.")
    {
    }

    public CreationModificationDateTimeException(string? message) : base(message)
    {
    }

    public CreationModificationDateTimeException(string? message, Exception? innerException) : base(message,
        innerException)
    {
    }

    public CreationModificationDateTimeException(string? message, string? paramName) : base(message, paramName)
    {
    }

    public CreationModificationDateTimeException(string? message, string? paramName, Exception? innerException) : base(
        message, paramName, innerException)
    {
    }

    protected CreationModificationDateTimeException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
    }
}