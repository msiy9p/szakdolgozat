using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct ReadingId : ICustomIdType<Guid>, IComparable<ReadingId>, IEquatable<ReadingId>
{
    public Guid Value { get; init; }

    public ReadingId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public ReadingId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static ReadingId Create() => new(Uuid.NewDatabaseFriendly());

    public static ReadingId? Convert(Guid? value) => value.HasValue ? new ReadingId(value.Value) : null;

    public int CompareTo(ReadingId other) => Value.CompareTo(other.Value);

    public bool Equals(ReadingId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is ReadingId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(ReadingId a, ReadingId b) => a.CompareTo(b) == 0;

    public static bool operator !=(ReadingId a, ReadingId b) => !(a == b);

    public static bool operator <(ReadingId left, ReadingId right) => left.CompareTo(right) < 0;

    public static bool operator <=(ReadingId left, ReadingId right) => left.CompareTo(right) <= 0;

    public static bool operator >(ReadingId left, ReadingId right) => left.CompareTo(right) > 0;

    public static bool operator >=(ReadingId left, ReadingId right) => left.CompareTo(right) >= 0;
}