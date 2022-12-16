using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using UUIDNext;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct NoteId : ICustomIdType<Guid>, IComparable<NoteId>, IEquatable<NoteId>
{
    public Guid Value { get; init; }

    public NoteId()
    {
        Value = Uuid.NewDatabaseFriendly();
    }

    public NoteId(Guid value)
    {
        Guard.Against.NullOrEmpty(value);
        Value = value;
    }

    public static NoteId Create() => new(Uuid.NewDatabaseFriendly());

    public static NoteId? Convert(Guid? value) => value.HasValue ? new NoteId(value.Value) : null;

    public int CompareTo(NoteId other) => Value.CompareTo(other.Value);

    public bool Equals(NoteId other) => Value.Equals(other.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is NoteId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format) => Value.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

    public static bool operator ==(NoteId a, NoteId b) => a.CompareTo(b) == 0;

    public static bool operator !=(NoteId a, NoteId b) => !(a == b);

    public static bool operator <(NoteId left, NoteId right) => left.CompareTo(right) < 0;

    public static bool operator <=(NoteId left, NoteId right) => left.CompareTo(right) <= 0;

    public static bool operator >(NoteId left, NoteId right) => left.CompareTo(right) > 0;

    public static bool operator >=(NoteId left, NoteId right) => left.CompareTo(right) >= 0;
}