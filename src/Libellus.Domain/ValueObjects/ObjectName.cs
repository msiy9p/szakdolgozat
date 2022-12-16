using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class ObjectName : BaseValueObject, IComparable, IComparable<ObjectName>, IEquatable<ObjectName>
{
    public const int MaxLength = 1024;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public ObjectName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        if (name.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(name));
        }

        Value = name.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<ObjectName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ObjectNameErrors.InvalidObjectName.ToInvalidResult<ObjectName>();
        }

        if (name.Length > MaxLength)
        {
            return ObjectNameErrors.ObjectNameTooLong.ToInvalidResult<ObjectName>();
        }

        return Result<ObjectName>.Success(new ObjectName(name));
    }

    public static Result Probe(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ObjectNameErrors.InvalidObjectName.ToInvalidResult();
        }

        if (name.Length > MaxLength)
        {
            return ObjectNameErrors.ObjectNameTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public override string ToString()
    {
        return Value;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(ObjectName? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is ObjectName item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(ObjectName? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(ObjectName item) => item.Value;
    public static explicit operator ObjectName(string item) => new(item);

    public static bool operator ==(ObjectName left, ObjectName right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(ObjectName left, ObjectName right) => !(left == right);

    public static bool operator <(ObjectName left, ObjectName right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(ObjectName left, ObjectName right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(ObjectName left, ObjectName right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(ObjectName left, ObjectName right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByObjectName : Comparer<ObjectName>
    {
        public override int Compare(ObjectName? x, ObjectName? y)
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

    public class CompareByNameDesc : Comparer<ObjectName>
    {
        public override int Compare(ObjectName? x, ObjectName? y)
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