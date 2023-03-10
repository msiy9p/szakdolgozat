using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct TagId : ICustomIdType<Guid>, IComparable<TagId>, IEquatable<TagId>
{
    public Guid Value { get; init; }

    public TagId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public TagId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static TagId Create() => new(Uuid.NewDatabaseFriendly());

    public static TagId? Convert(Guid? value) => value.HasValue ? new TagId(value.Value) : null;

    public int CompareTo(TagId other) => Value.CompareTo(other.Value);

    public bool Equals(TagId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is TagId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(TagId a, TagId b) => a.CompareTo(b) == 0;

    public static bool operator !=(TagId a, TagId b) => !(a == b);

    public static bool operator <(TagId left, TagId right) => left.CompareTo(right) < 0;

    public static bool operator <=(TagId left, TagId right) => left.CompareTo(right) <= 0;

    public static bool operator >(TagId left, TagId right) => left.CompareTo(right) > 0;

    public static bool operator >=(TagId left, TagId right) => left.CompareTo(right) >= 0;
}