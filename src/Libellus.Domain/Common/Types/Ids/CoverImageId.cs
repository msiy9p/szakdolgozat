using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct CoverImageId : ICustomIdType<Guid>, IComparable<CoverImageId>, IEquatable<CoverImageId>
{
    public Guid Value { get; init; }

    public CoverImageId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public CoverImageId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static CoverImageId Create() => new(Uuid.NewDatabaseFriendly());

    public static CoverImageId? Convert(Guid? value) => value.HasValue ? new CoverImageId(value.Value) : null;

    public int CompareTo(CoverImageId other) => Value.CompareTo(other.Value);

    public bool Equals(CoverImageId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is CoverImageId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(CoverImageId a, CoverImageId b) => a.CompareTo(b) == 0;

    public static bool operator !=(CoverImageId a, CoverImageId b) => !(a == b);

    public static bool operator <(CoverImageId left, CoverImageId right) => left.CompareTo(right) < 0;

    public static bool operator <=(CoverImageId left, CoverImageId right) => left.CompareTo(right) <= 0;

    public static bool operator >(CoverImageId left, CoverImageId right) => left.CompareTo(right) > 0;

    public static bool operator >=(CoverImageId left, CoverImageId right) => left.CompareTo(right) >= 0;
}