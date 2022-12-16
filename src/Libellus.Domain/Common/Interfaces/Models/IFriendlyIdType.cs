namespace Libellus.Domain.Common.Interfaces.Models;

public interface IFriendlyIdType
{
}

public interface IFriendlyIdType<T> : IFriendlyIdType, ICustomIdType<T> where T : IComparable<T>, IEquatable<T>
{
    static abstract bool IsValid(T? value);
}