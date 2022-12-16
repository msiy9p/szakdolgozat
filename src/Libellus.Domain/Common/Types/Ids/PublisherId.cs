using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct PublisherId : ICustomIdType<Guid>, IComparable<PublisherId>, IEquatable<PublisherId>
{
    public Guid Value { get; init; }

    public PublisherId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public PublisherId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static PublisherId Create() => new(Uuid.NewDatabaseFriendly());

    public static PublisherId? Convert(Guid? value) => value.HasValue ? new PublisherId(value.Value) : null;

    public int CompareTo(PublisherId other) => Value.CompareTo(other.Value);

    public bool Equals(PublisherId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is PublisherId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(PublisherId a, PublisherId b) => a.CompareTo(b) == 0;

    public static bool operator !=(PublisherId a, PublisherId b) => !(a == b);

    public static bool operator <(PublisherId left, PublisherId right) => left.CompareTo(right) < 0;

    public static bool operator <=(PublisherId left, PublisherId right) => left.CompareTo(right) <= 0;

    public static bool operator >(PublisherId left, PublisherId right) => left.CompareTo(right) > 0;

    public static bool operator >=(PublisherId left, PublisherId right) => left.CompareTo(right) >= 0;
}