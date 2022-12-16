using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct CommentId : ICustomIdType<Guid>, IComparable<CommentId>, IEquatable<CommentId>
{
    public Guid Value { get; init; }

    public CommentId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public CommentId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static CommentId Create() => new(Uuid.NewDatabaseFriendly());

    public static CommentId? Convert(Guid? value) => value.HasValue ? new CommentId(value.Value) : null;

    public int CompareTo(CommentId other) => Value.CompareTo(other.Value);

    public bool Equals(CommentId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is CommentId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(CommentId a, CommentId b) => a.CompareTo(b) == 0;

    public static bool operator !=(CommentId a, CommentId b) => !(a == b);

    public static bool operator <(CommentId left, CommentId right) => left.CompareTo(right) < 0;

    public static bool operator <=(CommentId left, CommentId right) => left.CompareTo(right) <= 0;

    public static bool operator >(CommentId left, CommentId right) => left.CompareTo(right) > 0;

    public static bool operator >=(CommentId left, CommentId right) => left.CompareTo(right) >= 0;
}