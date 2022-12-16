using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct GroupId : ICustomIdType<Guid>, IComparable<GroupId>, IEquatable<GroupId>
{
    public Guid Value { get; init; }

    public GroupId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public GroupId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static GroupId Create() => new(Uuid.NewDatabaseFriendly());

    public static GroupId? Convert(Guid? value) => value.HasValue ? new GroupId(value.Value) : null;

    public int CompareTo(GroupId other) => Value.CompareTo(other.Value);

    public bool Equals(GroupId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is GroupId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(GroupId a, GroupId b) => a.CompareTo(b) == 0;

    public static bool operator !=(GroupId a, GroupId b) => !(a == b);

    public static bool operator <(GroupId left, GroupId right) => left.CompareTo(right) < 0;

    public static bool operator <=(GroupId left, GroupId right) => left.CompareTo(right) <= 0;

    public static bool operator >(GroupId left, GroupId right) => left.CompareTo(right) > 0;

    public static bool operator >=(GroupId left, GroupId right) => left.CompareTo(right) >= 0;
}