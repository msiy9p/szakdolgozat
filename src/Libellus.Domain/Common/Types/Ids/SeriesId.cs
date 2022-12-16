using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct SeriesId : ICustomIdType<Guid>, IComparable<SeriesId>, IEquatable<SeriesId>
{
    public Guid Value { get; init; }

    public SeriesId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public SeriesId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static SeriesId Create() => new(Uuid.NewDatabaseFriendly());

    public static SeriesId? Convert(Guid? value) => value.HasValue ? new SeriesId(value.Value) : null;

    public int CompareTo(SeriesId other) => Value.CompareTo(other.Value);

    public bool Equals(SeriesId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is SeriesId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(SeriesId a, SeriesId b) => a.CompareTo(b) == 0;

    public static bool operator !=(SeriesId a, SeriesId b) => !(a == b);

    public static bool operator <(SeriesId left, SeriesId right) => left.CompareTo(right) < 0;

    public static bool operator <=(SeriesId left, SeriesId right) => left.CompareTo(right) <= 0;

    public static bool operator >(SeriesId left, SeriesId right) => left.CompareTo(right) > 0;

    public static bool operator >=(SeriesId left, SeriesId right) => left.CompareTo(right) >= 0;
}