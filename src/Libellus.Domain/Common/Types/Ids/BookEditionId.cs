using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct BookEditionId : ICustomIdType<Guid>, IComparable<BookEditionId>, IEquatable<BookEditionId>
{
    public Guid Value { get; init; }

    public BookEditionId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public BookEditionId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static BookEditionId Create() => new(Uuid.NewDatabaseFriendly());

    public static BookEditionId? Convert(Guid? value) => value.HasValue ? new BookEditionId(value.Value) : null;

    public int CompareTo(BookEditionId other) => Value.CompareTo(other.Value);

    public bool Equals(BookEditionId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is BookEditionId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(BookEditionId a, BookEditionId b) => a.CompareTo(b) == 0;

    public static bool operator !=(BookEditionId a, BookEditionId b) => !(a == b);

    public static bool operator <(BookEditionId left, BookEditionId right) => left.CompareTo(right) < 0;

    public static bool operator <=(BookEditionId left, BookEditionId right) => left.CompareTo(right) <= 0;

    public static bool operator >(BookEditionId left, BookEditionId right) => left.CompareTo(right) > 0;

    public static bool operator >=(BookEditionId left, BookEditionId right) => left.CompareTo(right) >= 0;
}