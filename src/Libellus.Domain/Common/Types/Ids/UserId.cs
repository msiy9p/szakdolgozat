using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct UserId : ICustomIdType<Guid>, IComparable<UserId>, IEquatable<UserId>
{
    public Guid Value { get; init; }

    public UserId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public UserId(Guid value)
    {
        Value = value;
    }

    public static UserId Create() => new(Uuid.NewDatabaseFriendly());

    public static UserId? Convert(Guid? value) => value.HasValue ? new UserId(value.Value) : null;

    public int CompareTo(UserId other) => Value.CompareTo(other.Value);

    public bool Equals(UserId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is UserId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(UserId a, UserId b) => a.CompareTo(b) == 0;

    public static bool operator !=(UserId a, UserId b) => !(a == b);

    public static bool operator <(UserId left, UserId right) => left.CompareTo(right) < 0;

    public static bool operator <=(UserId left, UserId right) => left.CompareTo(right) <= 0;

    public static bool operator >(UserId left, UserId right) => left.CompareTo(right) > 0;

    public static bool operator >=(UserId left, UserId right) => left.CompareTo(right) >= 0;
}