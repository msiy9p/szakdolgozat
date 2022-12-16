using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct ProfilePictureId : ICustomIdType<Guid>, IComparable<ProfilePictureId>,
    IEquatable<ProfilePictureId>
{
    public Guid Value { get; init; }

    public ProfilePictureId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public ProfilePictureId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static ProfilePictureId Create() => new(Uuid.NewDatabaseFriendly());

    public static ProfilePictureId? Convert(Guid? value) => value.HasValue ? new ProfilePictureId(value.Value) : null;

    public int CompareTo(ProfilePictureId other) => Value.CompareTo(other.Value);

    public bool Equals(ProfilePictureId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is ProfilePictureId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(ProfilePictureId a, ProfilePictureId b) => a.CompareTo(b) == 0;

    public static bool operator !=(ProfilePictureId a, ProfilePictureId b) => !(a == b);

    public static bool operator <(ProfilePictureId left, ProfilePictureId right) => left.CompareTo(right) < 0;

    public static bool operator <=(ProfilePictureId left, ProfilePictureId right) => left.CompareTo(right) <= 0;

    public static bool operator >(ProfilePictureId left, ProfilePictureId right) => left.CompareTo(right) > 0;

    public static bool operator >=(ProfilePictureId left, ProfilePictureId right) => left.CompareTo(right) >= 0;
}