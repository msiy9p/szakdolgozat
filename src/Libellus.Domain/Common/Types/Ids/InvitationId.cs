using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct InvitationId : ICustomIdType<Guid>, IComparable<InvitationId>, IEquatable<InvitationId>
{
    public Guid Value { get; init; }

    public InvitationId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public InvitationId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static InvitationId Create() => new(Uuid.NewDatabaseFriendly());

    public static InvitationId? Convert(Guid? value) => value.HasValue ? new InvitationId(value.Value) : null;

    public int CompareTo(InvitationId other) => Value.CompareTo(other.Value);

    public bool Equals(InvitationId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is InvitationId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(InvitationId a, InvitationId b) => a.CompareTo(b) == 0;

    public static bool operator !=(InvitationId a, InvitationId b) => !(a == b);

    public static bool operator <(InvitationId left, InvitationId right) => left.CompareTo(right) < 0;

    public static bool operator <=(InvitationId left, InvitationId right) => left.CompareTo(right) <= 0;

    public static bool operator >(InvitationId left, InvitationId right) => left.CompareTo(right) > 0;

    public static bool operator >=(InvitationId left, InvitationId right) => left.CompareTo(right) >= 0;
}