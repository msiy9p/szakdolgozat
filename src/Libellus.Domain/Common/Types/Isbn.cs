using Ardalis.GuardClauses;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using System.Globalization;

namespace Libellus.Domain.Common.Types;

public readonly struct Isbn : IComparable, IComparable<Isbn>, IEquatable<Isbn>
{
    public string Value { get; init; }
    public IsbnType IsbnType { get; init; }

    public Isbn(string isbn)
    {
        Guard.Against.NullOrWhiteSpace(isbn);
        IsbnType? type = GetIsbnType(isbn);
        if (!type.HasValue)
        {
            throw new IsbnInvalidException("Not valid ISBN.", nameof(isbn));
        }

        Value = isbn.Replace("-", string.Empty).ToUpperInvariant();
        IsbnType = type.Value;
    }

    public static Result<Isbn> Create(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
        {
            return Result<Isbn>.Invalid(DomainErrors.IsbnErrors.InvalidIsbn);
        }

        IsbnType? type = GetIsbnType(isbn);
        if (!type.HasValue)
        {
            return Result<Isbn>.Invalid(DomainErrors.IsbnErrors.InvalidIsbn);
        }

        return Result<Isbn>.Success(new Isbn(isbn));
    }

    public static Isbn? Convert(string? isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
        {
            return null;
        }

        IsbnType? type = GetIsbnType(isbn);
        if (!type.HasValue)
        {
            return null;
        }

        return new Isbn(isbn);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Isbn isbn)
        {
            return CompareTo(isbn);
        }

        return 1;
    }

    public int CompareTo(Isbn other) =>
        string.Compare(Value, other.Value, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);

    public bool Equals(Isbn other)
    {
        if (IsbnType != other.IsbnType)
        {
            return false;
        }

        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return obj is Isbn other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(Isbn isbn) => isbn.Value;

    public static bool operator ==(Isbn a, Isbn b) => a.CompareTo(b) == 0;

    public static bool operator !=(Isbn a, Isbn b) => !(a == b);

    public static bool operator <(Isbn left, Isbn right) => left.CompareTo(right) < 0;

    public static bool operator <=(Isbn left, Isbn right) => left.CompareTo(right) <= 0;

    public static bool operator >(Isbn left, Isbn right) => left.CompareTo(right) > 0;

    public static bool operator >=(Isbn left, Isbn right) => left.CompareTo(right) >= 0;

    public static IsbnType? GetIsbnType(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
        {
            return null;
        }

        string temp = isbn.Replace("-", string.Empty).ToUpperInvariant();

        return temp.Length switch
        {
            10 => IsValidIsbn10(temp) ? IsbnType.Isbn10 : null,
            13 => IsValidIsbn13(temp) ? IsbnType.Isbn13 : null,
            _ => null
        };
    }

    private static bool IsValidIsbn10(string isbn10)
    {
        if (string.IsNullOrWhiteSpace(isbn10))
        {
            return false;
        }

        string temp = isbn10.Replace("-", string.Empty).ToUpperInvariant();

        if (temp.Length != 10)
        {
            return false;
        }

        if (!long.TryParse(temp.Substring(0, temp.Length - 1), out var _))
        {
            return false;
        }

        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (temp[i] - '0') * (i + 1);
        }

        bool result = false;
        int remainder = sum % 11;
        var lastChar = temp[temp.Length - 1];

        if (lastChar == 'X')
        {
            result = (remainder == 10);
        }
        else if (int.TryParse(lastChar.ToString(), out _))
        {
            result = (remainder == lastChar - '0');
        }

        return result;
    }

    private static bool IsValidIsbn13(string isbn13)
    {
        if (string.IsNullOrWhiteSpace(isbn13))
        {
            return false;
        }

        string temp = isbn13.Replace("-", string.Empty).ToUpperInvariant();

        if (temp.Length != 13)
        {
            return false;
        }

        if (!long.TryParse(temp, out _))
        {
            return false;
        }

        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            sum += (temp[i] - '0') * (i % 2 == 1 ? 3 : 1);
        }

        int remainder = sum % 10;
        int checkDigit = 10 - remainder;
        if (checkDigit == 10)
        {
            checkDigit = 0;
        }

        bool result = (checkDigit == temp[12] - '0');

        return result;
    }
}