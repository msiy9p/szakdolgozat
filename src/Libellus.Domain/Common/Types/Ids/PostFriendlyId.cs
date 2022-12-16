using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;

namespace Libellus.Domain.Common.Types.Ids;

public readonly struct PostFriendlyId : IFriendlyIdType<string>, IComparable<PostFriendlyId>, IEquatable<PostFriendlyId>
{
    public const int Length = 12;
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string Value { get; init; }

    public PostFriendlyId()
    {
        Value = Nanoid.Nanoid.Generate(Alphabet, Length);
    }

    public PostFriendlyId(string value)
    {
        Guard.Against.NullOrEmpty(value);
        if (value.Length != Length)
        {
            throw new ArgumentException("Length not valid.", nameof(value));
        }

        foreach (var item in value)
        {
            if (!Alphabet.Contains(item))
            {
                throw new ArgumentException("Not in alphabet.", nameof(value));
            }
        }

        Value = value;
    }

    public static PostFriendlyId Create() => new(Nanoid.Nanoid.Generate(Alphabet, Length));

    public static PostFriendlyId? Convert(string? value)
    {
        if (IsValid(value))
        {
            return new PostFriendlyId(value!);
        }

        return null;
    }

    public int CompareTo(PostFriendlyId other) => string.Compare(Value, other.Value, StringComparison.Ordinal);

    public bool Equals(PostFriendlyId other) => Value.Equals(other.Value);

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (value.Length != Length)
        {
            return false;
        }

        foreach (var item in value)
        {
            if (!Alphabet.Contains(item))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is PostFriendlyId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(PostFriendlyId a, PostFriendlyId b) => a.CompareTo(b) == 0;

    public static bool operator !=(PostFriendlyId a, PostFriendlyId b) => !(a == b);

    public static bool operator <(PostFriendlyId left, PostFriendlyId right) => left.CompareTo(right) < 0;

    public static bool operator <=(PostFriendlyId left, PostFriendlyId right) => left.CompareTo(right) <= 0;

    public static bool operator >(PostFriendlyId left, PostFriendlyId right) => left.CompareTo(right) > 0;

    public static bool operator >=(PostFriendlyId left, PostFriendlyId right) => left.CompareTo(right) >= 0;
}