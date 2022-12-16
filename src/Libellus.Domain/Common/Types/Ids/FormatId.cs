using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct FormatId : ICustomIdType<Guid>, IComparable<FormatId>, IEquatable<FormatId>
{
    public Guid Value { get; init; }

    public FormatId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public FormatId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static FormatId Create() => new(Uuid.NewDatabaseFriendly());

    public static FormatId? Convert(Guid? value) => value.HasValue ? new FormatId(value.Value) : null;

    public int CompareTo(FormatId other) => Value.CompareTo(other.Value);

    public bool Equals(FormatId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is FormatId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(FormatId a, FormatId b) => a.CompareTo(b) == 0;

    public static bool operator !=(FormatId a, FormatId b) => !(a == b);

    public static bool operator <(FormatId left, FormatId right) => left.CompareTo(right) < 0;

    public static bool operator <=(FormatId left, FormatId right) => left.CompareTo(right) <= 0;

    public static bool operator >(FormatId left, FormatId right) => left.CompareTo(right) > 0;

    public static bool operator >=(FormatId left, FormatId right) => left.CompareTo(right) >= 0;
}