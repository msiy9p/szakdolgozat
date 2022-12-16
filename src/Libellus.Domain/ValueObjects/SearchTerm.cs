using Ardalis.GuardClauses;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using System.Globalization;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.ValueObjects;

public sealed class SearchTerm : BaseValueObject, IEquatable<SearchTerm>
{
    public const int MaxLength = 250;

    public string Value { get; init; }
    public string ValueNormalized { get; init; }

    public SearchTerm(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        if (value.Length > MaxLength)
        {
            throw new ArgumentException("Longer then max length.", nameof(value));
        }

        Value = value.Trim();
        ValueNormalized = Value.ToNormalizedUpperInvariant();
    }

    public static Result<SearchTerm> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return SearchTermErrors.InvalidSearchTerm.ToInvalidResult<SearchTerm>();
        }

        if (value.Length > MaxLength)
        {
            return SearchTermErrors.SearchTermTooLong.ToInvalidResult<SearchTerm>();
        }

        return Result<SearchTerm>.Success(new SearchTerm(value));
    }

    public static Result Probe(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return SearchTermErrors.InvalidSearchTerm.ToInvalidResult();
        }

        if (value.Length > MaxLength)
        {
            return SearchTermErrors.SearchTermTooLong.ToInvalidResult();
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueNormalized;
    }

    public static bool IsValidSearchTerm(string value)
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

    public bool Equals(SearchTerm? other)
    {
        if (other is null)
        {
            return false;
        }

        return base.Equals(other);
    }

    public int CompareTo(object? obj)
    {
        if (obj is SearchTerm item)
        {
            return CompareTo(item);
        }

        return -1;
    }

    public int CompareTo(SearchTerm? other)
    {
        if (other is null)
        {
            return -1;
        }

        return string.Compare(ValueNormalized, other.ValueNormalized, CultureInfo.InvariantCulture,
            CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreSymbols);
    }

    public static implicit operator string(SearchTerm item) => item.Value;
    public static explicit operator SearchTerm(string item) => new(item);

    public static bool operator ==(SearchTerm left, SearchTerm right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(SearchTerm left, SearchTerm right) => !(left == right);
}