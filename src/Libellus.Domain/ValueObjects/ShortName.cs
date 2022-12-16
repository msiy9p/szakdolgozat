using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class ShortName : BaseValueObject, IComparable, IComparable<ShortName>, IEquatable<ShortName>
{
    public const int MaxLength = 50;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public ShortName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        if (name.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(name));
        }

        Value = name.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<ShortName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ShortNameErrors.InvalidShortName.ToInvalidResult<ShortName>();
        }

        if (name.Length > MaxLength)
        {
            return ShortNameErrors.ShortNameTooLong.ToInvalidResult<ShortName>();
        }

        return Result<ShortName>.Success(new ShortName(name));
    }

    public static Result Probe(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ShortNameErrors.InvalidShortName.ToInvalidResult();
        }

        if (name.Length > MaxLength)
        {
            return ShortNameErrors.ShortNameTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidShortName(string value)
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

    public bool Equals(ShortName? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is ShortName item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(ShortName? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(ShortName item) => item.Value;
    public static explicit operator ShortName(string item) => new(item);

    public static bool operator ==(ShortName left, ShortName right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(ShortName left, ShortName right) => !(left == right);

    public static bool operator <(ShortName left, ShortName right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(ShortName left, ShortName right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(ShortName left, ShortName right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(ShortName left, ShortName right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByName : Comparer<ShortName>
    {
        public override int Compare(ShortName? x, ShortName? y)
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

    public class CompareByNameDesc : Comparer<ShortName>
    {
        public override int Compare(ShortName? x, ShortName? y)
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