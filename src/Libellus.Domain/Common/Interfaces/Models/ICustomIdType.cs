namespace Libellus.Domain.Common.Interfaces.Models;

public interface ICustomIdType
{
}

public interface ICustomIdType<out T> : ICustomIdType where T : IComparable<T>, IEquatable<T>
{
    T Value { get; }
}