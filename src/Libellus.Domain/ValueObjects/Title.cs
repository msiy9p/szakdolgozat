using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class Title : BaseValueObject, IComparable, IComparable<Title>, IEquatable<Title>
{
    public const int MaxLength = 250;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public Title(string title)
    {
        Guard.Against.NullOrWhiteSpace(title);
        if (title.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(title));
        }

        Value = title.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<Title> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return TitleErrors.InvalidTitle.ToInvalidResult<Title>();
        }

        if (title.Length > MaxLength)
        {
            return TitleErrors.TitleTooLong.ToInvalidResult<Title>();
        }

        return Result<Title>.Success(new Title(title));
    }

    public static Result Probe(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return TitleErrors.InvalidTitle.ToInvalidResult();
        }

        if (title.Length > MaxLength)
        {
            return TitleErrors.TitleTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (value.Length > MaxLength)
        {
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return Value;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(Title? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Title item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(Title? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(Title item) => item.Value;
    public static explicit operator Title(string item) => new(item);

    public static bool operator ==(Title left, Title right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Title left, Title right) => !(left == right);

    public static bool operator <(Title left, Title right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Title left, Title right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Title left, Title right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Title left, Title right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByTitle : Comparer<Title>
    {
        public override int Compare(Title? x, Title? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            return x!.CompareTo(y);
        }
    }

    public class CompareByTitleDesc : Comparer<Title>
    {
        public override int Compare(Title? x, Title? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            return x!.CompareTo(y) * -1;
        }
    }
}