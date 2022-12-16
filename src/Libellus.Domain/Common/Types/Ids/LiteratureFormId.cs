using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct LiteratureFormId : ICustomIdType<Guid>, IComparable<LiteratureFormId>,
    IEquatable<LiteratureFormId>
{
    public Guid Value { get; init; }

    public LiteratureFormId()
    {
        Value = Guid.NewGuid();
    }

    public LiteratureFormId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static LiteratureFormId Create() => new(Guid.NewGuid());

    public static LiteratureFormId? Convert(Guid? value) => value.HasValue ? new LiteratureFormId(value.Value) : null;

    public int CompareTo(LiteratureFormId other) => Value.CompareTo(other.Value);

    public bool Equals(LiteratureFormId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is LiteratureFormId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(LiteratureFormId a, LiteratureFormId b) => a.CompareTo(b) == 0;

    public static bool operator !=(LiteratureFormId a, LiteratureFormId b) => !(a == b);

    public static bool operator <(LiteratureFormId left, LiteratureFormId right) => left.CompareTo(right) < 0;

    public static bool operator <=(LiteratureFormId left, LiteratureFormId right) => left.CompareTo(right) <= 0;

    public static bool operator >(LiteratureFormId left, LiteratureFormId right) => left.CompareTo(right) > 0;

    public static bool operator >=(LiteratureFormId left, LiteratureFormId right) => left.CompareTo(right) >= 0;
}