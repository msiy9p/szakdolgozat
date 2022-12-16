using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct PostId : ICustomIdType<Guid>, IComparable<PostId>, IEquatable<PostId>
{
    public Guid Value { get; init; }

    public PostId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public PostId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static PostId Create() => new(Uuid.NewDatabaseFriendly());

    public static PostId? Convert(Guid? value) => value.HasValue ? new PostId(value.Value) : null;

    public int CompareTo(PostId other) => Value.CompareTo(other.Value);

    public bool Equals(PostId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is PostId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(PostId a, PostId b) => a.CompareTo(b) == 0;

    public static bool operator !=(PostId a, PostId b) => !(a == b);

    public static bool operator <(PostId left, PostId right) => left.CompareTo(right) < 0;

    public static bool operator <=(PostId left, PostId right) => left.CompareTo(right) <= 0;

    public static bool operator >(PostId left, PostId right) => left.CompareTo(right) > 0;

    public static bool operator >=(PostId left, PostId right) => left.CompareTo(right) >= 0;
}