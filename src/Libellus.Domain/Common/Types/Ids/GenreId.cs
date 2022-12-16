using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct GenreId : ICustomIdType<Guid>, IComparable<GenreId>, IEquatable<GenreId>
{
    public Guid Value { get; init; }

    public GenreId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public GenreId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static GenreId Create() => new(Uuid.NewDatabaseFriendly());

    public static GenreId? Convert(Guid? value) => value.HasValue ? new GenreId(value.Value) : null;

    public int CompareTo(GenreId other) => Value.CompareTo(other.Value);

    public bool Equals(GenreId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is GenreId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(GenreId a, GenreId b) => a.CompareTo(b) == 0;

    public static bool operator !=(GenreId a, GenreId b) => !(a == b);

    public static bool operator <(GenreId left, GenreId right) => left.CompareTo(right) < 0;

    public static bool operator <=(GenreId left, GenreId right) => left.CompareTo(right) <= 0;

    public static bool operator >(GenreId left, GenreId right) => left.CompareTo(right) > 0;

    public static bool operator >=(GenreId left, GenreId right) => left.CompareTo(right) >= 0;
}