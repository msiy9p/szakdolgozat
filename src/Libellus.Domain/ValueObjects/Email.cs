using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class Email : BaseValueObject, IComparable, IComparable<Email>, IEquatable<Email>
{
    public const int MaxLength = 256;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public Email(string email)
    {
        Guard.Against.NullOrWhiteSpace(email);
        if (email.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(email));
        }

        Value = email.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return EmailErrors.InvalidEmail.ToInvalidResult<Email>();
        }

        if (email.Length > MaxLength)
        {
            return EmailErrors.EmailTooLong.ToInvalidResult<Email>();
        }

        return Result<Email>.Success(new Email(email));
    }

    public static Result Probe(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return EmailErrors.InvalidEmail.ToInvalidResult();
        }

        if (email.Length > MaxLength)
        {
            return EmailErrors.EmailTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidEmail(string value)
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

    public bool Equals(Email? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Email item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(Email? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(Email item) => item.Value;
    public static explicit operator Email(string item) => new(item);

    public static bool operator ==(Email left, Email right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Email left, Email right) => !(left == right);

    public static bool operator <(Email left, Email right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Email left, Email right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Email left, Email right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Email left, Email right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    public class CompareByEmail : Comparer<Email>
    {
        public override int Compare(Email? x, Email? y)
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

    public class CompareByEmailDesc : Comparer<Email>
    {
        public override int Compare(Email? x, Email? y)
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