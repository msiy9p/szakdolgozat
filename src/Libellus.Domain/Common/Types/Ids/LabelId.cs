using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct LabelId : ICustomIdType<Guid>, IComparable<LabelId>, IEquatable<LabelId>
{
    public Guid Value { get; init; }

    public LabelId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public LabelId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static LabelId Create() => new(Uuid.NewDatabaseFriendly());

    public static LabelId? Convert(Guid? value) => value.HasValue ? new LabelId(value.Value) : null;

    public int CompareTo(LabelId other) => Value.CompareTo(other.Value);

    public bool Equals(LabelId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is LabelId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(LabelId a, LabelId b) => a.CompareTo(b) == 0;

    public static bool operator !=(LabelId a, LabelId b) => !(a == b);

    public static bool operator <(LabelId left, LabelId right) => left.CompareTo(right) < 0;

    public static bool operator <=(LabelId left, LabelId right) => left.CompareTo(right) <= 0;

    public static bool operator >(LabelId left, LabelId right) => left.CompareTo(right) > 0;

    public static bool operator >=(LabelId left, LabelId right) => left.CompareTo(right) >= 0;
}