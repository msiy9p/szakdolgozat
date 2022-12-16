using System.ComponentModel;
using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class ImageFormatNotSupportedException : InvalidEnumArgumentException
{
    public ImageFormatNotSupportedException()
    {
    }

    public ImageFormatNotSupportedException(string? message) : base(message)
    {
    }

    public ImageFormatNotSupportedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ImageFormatNotSupportedException(string? argumentName, int invalidValue, Type enumClass) : base(argumentName,
        invalidValue, enumClass)
    {
    }

    protected ImageFormatNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}