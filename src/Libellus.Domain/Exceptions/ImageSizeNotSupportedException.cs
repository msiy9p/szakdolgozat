using System.ComponentModel;
using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class ImageSizeNotSupportedException : InvalidEnumArgumentException
{
    public ImageSizeNotSupportedException()
    {
    }

    public ImageSizeNotSupportedException(string? message) : base(message)
    {
    }

    public ImageSizeNotSupportedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ImageSizeNotSupportedException(string? argumentName, int invalidValue, Type enumClass) : base(argumentName,
        invalidValue, enumClass)
    {
    }

    protected ImageSizeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}