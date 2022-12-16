using System.ComponentModel;
using System.Runtime.Serialization;

namespace Libellus.Domain.Exceptions;

[Serializable]
public class GroupRoleInvalidException : InvalidEnumArgumentException
{
    public GroupRoleInvalidException()
    {
    }

    public GroupRoleInvalidException(string? message) : base(message)
    {
    }

    public GroupRoleInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public GroupRoleInvalidException(string? argumentName, int invalidValue, Type enumClass) : base(argumentName,
        invalidValue, enumClass)
    {
    }

    protected GroupRoleInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}