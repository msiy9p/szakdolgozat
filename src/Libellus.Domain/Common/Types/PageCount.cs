using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Types;

public readonly struct PageCount : IEquatable<PageCount>, IComparable, IComparable<PageCount>
{
    public const int MinimumPageCount = 1;

    public int Value { get; init; }

    public PageCount(int value)
    {
        if (!IsValidPageCount(value))
        {
            throw new PageCountInvalidException("Invalid page count.", nameof(value));
        }

        Value = value;
    }

    public static Result<PageCount> Create(int value)
    {
        if (!IsValidPageCount(value))
        {
            return Result<PageCount>.Invalid(PageCountErrors.InvalidPageCount);
        }

        return Result<PageCount>.Success(new PageCount(value));
    }

    public static PageCount? Convert(int? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        if (!IsValidPageCount(value.Value))
        {
            return null;
        }

        return new PageCount(value.Value);
    }

    private static bool IsValidPageCount(int pageCount) => pageCount >= MinimumPageCount;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object? obj) => obj is PageCount other && Equals(other);

    public bool Equals(PageCount other) => Value.Equals(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is PageCount pageCount)
        {
            return CompareTo(pageCount);
        }

        return 1;
    }

    public int CompareTo(PageCount other) => Value.CompareTo(other.Value);

    public static implicit operator int(PageCount pageCount) => pageCount.Value;

    public static bool operator ==(PageCount left, PageCount right) => left.Equals(right);

    public static bool operator !=(PageCount left, PageCount right) => !(left == right);

    public static bool operator <(PageCount left, PageCount right) => left.CompareTo(right) < 0;

    public static bool operator <=(PageCount left, PageCount right) => left.CompareTo(right) <= 0;

    public static bool operator >(PageCount left, PageCount right) => left.CompareTo(right) > 0;

    public static bool operator >=(PageCount left, PageCount right) => left.CompareTo(right) >= 0;
}