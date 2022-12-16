using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct AuthorId : ICustomIdType<Guid>, IComparable<AuthorId>, IEquatable<AuthorId>
{
    public Guid Value { get; init; }

    public AuthorId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public AuthorId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static AuthorId Create() => new(Uuid.NewDatabaseFriendly());

    public static AuthorId? Convert(Guid? value) => value.HasValue ? new AuthorId(value.Value) : null;

    public int CompareTo(AuthorId other) => Value.CompareTo(other.Value);

    public bool Equals(AuthorId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is AuthorId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(AuthorId a, AuthorId b) => a.CompareTo(b) == 0;

    public static bool operator !=(AuthorId a, AuthorId b) => !(a == b);

    public static bool operator <(AuthorId left, AuthorId right) => left.CompareTo(right) < 0;

    public static bool operator <=(AuthorId left, AuthorId right) => left.CompareTo(right) <= 0;

    public static bool operator >(AuthorId left, AuthorId right) => left.CompareTo(right) > 0;

    public static bool operator >=(AuthorId left, AuthorId right) => left.CompareTo(right) >= 0;
}