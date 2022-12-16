using Ardalis.GuardClauses;
using Libellus.Application.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using static Libellus.Application.Errors.ApplicationErrors;

namespace Libellus.Application.Models;

public readonly struct PaginationInfo : IEquatable<PaginationInfo>
{
    public static readonly PaginationInfo Default = new();

    public const int DefaultPageNumber = 1;
    public const PaginationItemCount DefaultItemCount = PaginationItemCount.Items25;

    public int PageNumber { get; init; }
    public PaginationItemCount ItemCount { get; init; }

    public PaginationInfo() : this(DefaultPageNumber, DefaultItemCount)
    {
    }

    public PaginationInfo(int pageNumber, int itemCount) : this(pageNumber, (PaginationItemCount)itemCount)
    {
    }

    public PaginationInfo(int pageNumber, PaginationItemCount itemCount)
    {
        PageNumber = Guard.Against.NegativeOrZero(pageNumber);
        ItemCount = Guard.Against.PaginationItemCountOutOfRange(itemCount);
    }

    public static Result<PaginationInfo> Create(int pageNumber, int itemCount, bool adjustItemCount)
    {
        return Create(pageNumber, (PaginationItemCount)itemCount, adjustItemCount);
    }

    public static Result<PaginationInfo> Create(int pageNumber, PaginationItemCount itemCount, bool adjustItemCount)
    {
        if (pageNumber < 1)
        {
            return PaginationInfoErrors.InvalidPageNumber.ToInvalidResult<PaginationInfo>();
        }

        if (!PaginationItemCountExtensions.IsDefined(itemCount) && !adjustItemCount)
        {
            return PaginationInfoErrors.InvalidItemCount.ToInvalidResult<PaginationInfo>();
        }

        if (!PaginationItemCountExtensions.IsDefined(itemCount) && adjustItemCount)
        {
            var nearest = PaginationItemCountExtensions.GetAll()
                .MinBy(x => Math.Abs((int)x - (int)itemCount));

            return Result<PaginationInfo>.Success(new PaginationInfo(pageNumber, nearest));
        }

        return Result<PaginationInfo>.Success(new PaginationInfo(pageNumber, itemCount));
    }

    public PaginationInfo Adjust(in int totalCount) => Adjust(this, totalCount);

    public static PaginationInfo Adjust(PaginationInfo pagination, in int totalCount)
    {
        if (totalCount <= 0)
        {
            return new PaginationInfo(DefaultPageNumber, pagination.ItemCount);
        }

        if ((int)pagination.ItemCount * pagination.PageNumber <= totalCount)
        {
            return pagination;
        }

        if (IsWithinPagination(totalCount, (int)pagination.ItemCount, pagination.PageNumber))
        {
            return pagination;
        }

        var page = pagination.PageNumber;
        do
        {
            page--;
        } while (IsWithinPagination(totalCount, (int)pagination.ItemCount, page));

        return new PaginationInfo(page, pagination.ItemCount);
    }

    private static bool IsWithinPagination(in int totalCount, in int itemCount, in int page) =>
        ((itemCount * (page - 1)) + 1 <= totalCount) && (totalCount <= itemCount * page);

    public int GetSkip() => (PageNumber - 1) * (int)ItemCount;

    public int GetTake() => (int)ItemCount;

    public bool Equals(PaginationInfo other) => PageNumber == other.PageNumber && ItemCount == other.ItemCount;

    public override bool Equals(object? obj) => obj is PaginationInfo other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PageNumber, (int)ItemCount);

    public void Deconstruct(out int pageNumber, out PaginationItemCount itemCount)
    {
        pageNumber = PageNumber;
        itemCount = ItemCount;
    }

    public void Deconstruct(out int pageNumber, out int itemCount)
    {
        pageNumber = PageNumber;
        itemCount = (int)ItemCount;
    }

    public static bool operator ==(PaginationInfo left, PaginationInfo right) => left.Equals(right);

    public static bool operator !=(PaginationInfo left, PaginationInfo right) => !(left == right);
}