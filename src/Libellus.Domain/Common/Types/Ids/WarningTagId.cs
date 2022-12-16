using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct WarningTagId : ICustomIdType<Guid>, IComparable<WarningTagId>, IEquatable<WarningTagId>
{
    public Guid Value { get; init; }

    public WarningTagId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public WarningTagId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static WarningTagId Create() => new(Uuid.NewDatabaseFriendly());

    public static WarningTagId? Convert(Guid? value) => value.HasValue ? new WarningTagId(value.Value) : null;

    public int CompareTo(WarningTagId other) => Value.CompareTo(other.Value);

    public bool Equals(WarningTagId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is WarningTagId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(WarningTagId a, WarningTagId b) => a.CompareTo(b) == 0;

    public static bool operator !=(WarningTagId a, WarningTagId b) => !(a == b);

    public static bool operator <(WarningTagId left, WarningTagId right) => left.CompareTo(right) < 0;

    public static bool operator <=(WarningTagId left, WarningTagId right) => left.CompareTo(right) <= 0;

    public static bool operator >(WarningTagId left, WarningTagId right) => left.CompareTo(right) > 0;

    public static bool operator >=(WarningTagId left, WarningTagId right) => left.CompareTo(right) >= 0;
}