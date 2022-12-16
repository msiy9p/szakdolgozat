using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct CustomIdTemplate : ICustomIdType<Guid>, IComparable<CustomIdTemplate>,
    IEquatable<CustomIdTemplate>
{
    public Guid Value { get; init; }

    public CustomIdTemplate()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public CustomIdTemplate(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static CustomIdTemplate Create() => new(Uuid.NewDatabaseFriendly());

    public static CustomIdTemplate? Convert(Guid? value) => value.HasValue ? new CustomIdTemplate(value.Value) : null;

    public int CompareTo(CustomIdTemplate other) => Value.CompareTo(other.Value);

    public bool Equals(CustomIdTemplate other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is CustomIdTemplate other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(CustomIdTemplate a, CustomIdTemplate b) => a.CompareTo(b) == 0;

    public static bool operator !=(CustomIdTemplate a, CustomIdTemplate b) => !(a == b);

    public static bool operator <(CustomIdTemplate left, CustomIdTemplate right) => left.CompareTo(right) < 0;

    public static bool operator <=(CustomIdTemplate left, CustomIdTemplate right) => left.CompareTo(right) <= 0;

    public static bool operator >(CustomIdTemplate left, CustomIdTemplate right) => left.CompareTo(right) > 0;

    public static bool operator >=(CustomIdTemplate left, CustomIdTemplate right) => left.CompareTo(right) >= 0;
}