using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct ShelfId : ICustomIdType<Guid>, IComparable<ShelfId>, IEquatable<ShelfId>
{
    public Guid Value { get; init; }

    public ShelfId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public ShelfId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static ShelfId Create() => new(Uuid.NewDatabaseFriendly());

    public static ShelfId? Convert(Guid? value) => value.HasValue ? new ShelfId(value.Value) : null;

    public int CompareTo(ShelfId other) => Value.CompareTo(other.Value);

    public bool Equals(ShelfId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is ShelfId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(ShelfId a, ShelfId b) => a.CompareTo(b) == 0;

    public static bool operator !=(ShelfId a, ShelfId b) => !(a == b);

    public static bool operator <(ShelfId left, ShelfId right) => left.CompareTo(right) < 0;

    public static bool operator <=(ShelfId left, ShelfId right) => left.CompareTo(right) <= 0;

    public static bool operator >(ShelfId left, ShelfId right) => left.CompareTo(right) > 0;

    public static bool operator >=(ShelfId left, ShelfId right) => left.CompareTo(right) >= 0;
}