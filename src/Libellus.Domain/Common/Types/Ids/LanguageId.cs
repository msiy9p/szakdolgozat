using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct LanguageId : ICustomIdType<Guid>, IComparable<LanguageId>, IEquatable<LanguageId>
{
    public Guid Value { get; init; }

    public LanguageId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public LanguageId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static LanguageId Create() => new(Uuid.NewDatabaseFriendly());

    public static LanguageId? Convert(Guid? value) => value.HasValue ? new LanguageId(value.Value) : null;

    public int CompareTo(LanguageId other) => Value.CompareTo(other.Value);

    public bool Equals(LanguageId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is LanguageId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(LanguageId a, LanguageId b) => a.CompareTo(b) == 0;

    public static bool operator !=(LanguageId a, LanguageId b) => !(a == b);

    public static bool operator <(LanguageId left, LanguageId right) => left.CompareTo(right) < 0;

    public static bool operator <=(LanguageId left, LanguageId right) => left.CompareTo(right) <= 0;

    public static bool operator >(LanguageId left, LanguageId right) => left.CompareTo(right) > 0;

    public static bool operator >=(LanguageId left, LanguageId right) => left.CompareTo(right) >= 0;
}