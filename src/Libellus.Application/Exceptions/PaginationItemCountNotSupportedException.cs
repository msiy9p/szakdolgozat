using System.ComponentModel;
using System.Runtime.Serialization;

namespace Libellus.Application.Exceptions;

[Serializable]
public class PaginationItemCountNotSupportedException : InvalidEnumArgumentException
{
    public PaginationItemCountNotSupportedException()
    {
    }

    public PaginationItemCountNotSupportedException(string? message) : base(message)
    {
    }

    public PaginationItemCountNotSupportedException(string? message, Exception? innerException) : base(message,
        innerException)
    {
    }

    public PaginationItemCountNotSupportedException(string? argumentName, int invalidValue, Type enumClass) : base(
        argumentName, invalidValue, enumClass)
    {
    }

    protected PaginationItemCountNotSupportedException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
    }
}