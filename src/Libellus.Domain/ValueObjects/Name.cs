using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class Name : BaseValueObject, IComparable, IComparable<Name>, IEquatable<Name>
{
    public const int MaxLength = 250;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public Name(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        if (name.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(name));
        }

        Value = name.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return NameErrors.InvalidName.ToInvalidResult<Name>();
        }

        if (name.Length > MaxLength)
        {
            return NameErrors.NameTooLong.ToInvalidResult<Name>();
        }

        return Result<Name>.Success(new Name(name));
    }

    public static Result Probe(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return NameErrors.InvalidName.ToInvalidResult();
        }

        if (name.Length > MaxLength)
        {
            return NameErrors.NameTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidName(string value)
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

    public bool Equals(Name? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Name item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(Name? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(Name item) => item.Value;
    public static explicit operator Name(string item) => new(item);

    public static bool operator ==(Name left, Name right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Name left, Name right) => !(left == right);

    public static bool operator <(Name left, Name right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Name left, Name right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Name left, Name right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Name left, Name right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByName : Comparer<Name>
    {
        public override int Compare(Name? x, Name? y)
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

    public class CompareByNameDesc : Comparer<Name>
    {
        public override int Compare(Name? x, Name? y)
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