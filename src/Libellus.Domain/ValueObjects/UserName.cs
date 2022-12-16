using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class UserName : BaseValueObject, IComparable, IComparable<UserName>, IEquatable<UserName>
{
    public const int MaxLength = 256;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public UserName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        if (name.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(name));
        }

        Value = name.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<UserName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UserNameErrors.InvalidUserName.ToInvalidResult<UserName>();
        }

        if (name.Length > MaxLength)
        {
            return UserNameErrors.UserNameTooLong.ToInvalidResult<UserName>();
        }

        return Result<UserName>.Success(new UserName(name));
    }

    public static Result Probe(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UserNameErrors.InvalidUserName.ToInvalidResult();
        }

        if (name.Length > MaxLength)
        {
            return UserNameErrors.UserNameTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidUserName(string value)
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

    public bool Equals(UserName? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is UserName item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(UserName? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(UserName item) => item.Value;
    public static explicit operator UserName(string item) => new(item);

    public static bool operator ==(UserName left, UserName right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(UserName left, UserName right) => !(left == right);

    public static bool operator <(UserName left, UserName right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(UserName left, UserName right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(UserName left, UserName right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(UserName left, UserName right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByUserName : Comparer<UserName>
    {
        public override int Compare(UserName? x, UserName? y)
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

    public class CompareByUserNameDesc : Comparer<UserName>
    {
        public override int Compare(UserName? x, UserName? y)
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