using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Types;

public readonly struct WordCount : IEquatable<WordCount>, IComparable, IComparable<WordCount>
{
    public const int MinimumWordCount = 0;

    public int Value { get; init; }

    public WordCount(int value)
    {
        if (!IsValidWordCount(value))
        {
            throw new WordCountInvalidException("Invalid word count.", nameof(value));
        }

        Value = value;
    }

    public static Result<WordCount> Create(int value)
    {
        if (!IsValidWordCount(value))
        {
            return Result<WordCount>.Invalid(WordCountErrors.InvalidWordCount);
        }

        return Result<WordCount>.Success(new WordCount(value));
    }

    public static WordCount? Convert(int? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        if (!IsValidWordCount(value.Value))
        {
            return null;
        }

        return new WordCount(value.Value);
    }

    private static bool IsValidWordCount(int pageCount) => pageCount >= MinimumWordCount;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object? obj) => obj is WordCount other && Equals(other);

    public bool Equals(WordCount other) => Value.Equals(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is WordCount pageCount)
        {
            return CompareTo(pageCount);
        }

        return 1;
    }

    public int CompareTo(WordCount other) => Value.CompareTo(other.Value);

    public static implicit operator int(WordCount wordCount) => wordCount.Value;

    public static bool operator ==(WordCount left, WordCount right) => left.Equals(right);

    public static bool operator !=(WordCount left, WordCount right) => !(left == right);

    public static bool operator <(WordCount left, WordCount right) => left.CompareTo(right) < 0;

    public static bool operator <=(WordCount left, WordCount right) => left.CompareTo(right) <= 0;

    public static bool operator >(WordCount left, WordCount right) => left.CompareTo(right) > 0;

    public static bool operator >=(WordCount left, WordCount right) => left.CompareTo(right) >= 0;
}