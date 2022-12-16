using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class Password : BaseValueObject, IComparable, IComparable<Password>, IEquatable<Password>
{
    public const int MaxLength = 250;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public Password(string password)
    {
        Guard.Against.NullOrWhiteSpace(password);
        if (password.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(password));
        }

        Value = password.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<Password> Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordErrors.InvalidPassword.ToInvalidResult<Password>();
        }

        if (password.Length > MaxLength)
        {
            return PasswordErrors.PasswordTooLong.ToInvalidResult<Password>();
        }

        return Result<Password>.Success(new Password(password));
    }

    public static Result Probe(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordErrors.InvalidPassword.ToInvalidResult();
        }

        if (password.Length > MaxLength)
        {
            return PasswordErrors.PasswordTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidPassword(string value)
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

    public bool Equals(Password? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Password item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(Password? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(Password item) => item.Value;
    public static explicit operator Password(string item) => new(item);

    public static bool operator ==(Password left, Password right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Password left, Password right) => !(left == right);

    public static bool operator <(Password left, Password right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Password left, Password right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Password left, Password right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Password left, Password right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByPassword : Comparer<Password>
    {
        public override int Compare(Password? x, Password? y)
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

    public class CompareByPasswordDesc : Comparer<Password>
    {
        public override int Compare(Password? x, Password? y)
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