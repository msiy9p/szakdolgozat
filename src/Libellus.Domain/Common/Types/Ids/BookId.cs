using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct BookId : ICustomIdType<Guid>, IComparable<BookId>, IEquatable<BookId>
{
    public Guid Value { get; init; }

    public BookId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public BookId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static BookId Create() => new(Uuid.NewDatabaseFriendly());

    public static BookId? Convert(Guid? value) => value.HasValue ? new BookId(value.Value) : null;

    public int CompareTo(BookId other) => Value.CompareTo(other.Value);

    public bool Equals(BookId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is BookId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(BookId a, BookId b) => a.CompareTo(b) == 0;

    public static bool operator !=(BookId a, BookId b) => !(a == b);

    public static bool operator <(BookId left, BookId right) => left.CompareTo(right) < 0;

    public static bool operator <=(BookId left, BookId right) => left.CompareTo(right) <= 0;

    public static bool operator >(BookId left, BookId right) => left.CompareTo(right) > 0;

    public static bool operator >=(BookId left, BookId right) => left.CompareTo(right) >= 0;
}