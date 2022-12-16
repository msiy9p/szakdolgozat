using Ardalis.GuardClauses;
using Libellus.Application.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using static Libellus.Application.Errors.ApplicationErrors;

namespace Libellus.Application.Models;

public class PaginationDetail
{
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public int CurrentPageNumber { get; init; }
    public PaginationItemCount PaginationCount { get; init; }

    public bool HasPreviousPage => CurrentPageNumber > 1;

    public bool HasNextPage => CurrentPageNumber < TotalPages;

    public PaginationDetail(int totalCount, PaginationInfo paginationInfo) :
        this(totalCount, paginationInfo.PageNumber, paginationInfo.ItemCount)
    {
    }

    public PaginationDetail(int totalCount, int currentPageNumber, PaginationItemCount paginationCount)
    {
        TotalCount = Guard.Against.Negative(totalCount);
        CurrentPageNumber = Guard.Against.NegativeOrZero(currentPageNumber);
        PaginationCount = Guard.Against.PaginationItemCountOutOfRange(paginationCount);

        TotalPages = (int)Math.Ceiling(TotalCount / (double)PaginationCount);
    }

    public static Result<PaginationDetail> Create(int totalCount, PaginationInfo paginationInfo,
        bool adjustPaginationCount)
    {
        return Create(totalCount, paginationInfo.PageNumber, paginationInfo.ItemCount, adjustPaginationCount);
    }

    public static Result<PaginationDetail> Create(int totalCount, int currentPageNumber,
        PaginationItemCount paginationCount, bool adjustPaginationCount)
    {
        if (totalCount < 0)
        {
            return DomainErrors.GeneralErrors.InputIsNegative.ToInvalidResult<PaginationDetail>();
        }

        if (currentPageNumber <= 0)
        {
            return DomainErrors.GeneralErrors.InputIsNegativeOrZero.ToInvalidResult<PaginationDetail>();
        }

        if (!PaginationItemCountExtensions.IsDefined(paginationCount) && !adjustPaginationCount)
        {
            return PaginationInfoErrors.InvalidItemCount.ToInvalidResult<PaginationDetail>();
        }

        if (!PaginationItemCountExtensions.IsDefined(paginationCount) && adjustPaginationCount)
        {
            var nearest = PaginationItemCountExtensions.GetAll()
                .MinBy(x => Math.Abs((int)x - (int)paginationCount));

            return Result<PaginationDetail>.Success(
                new PaginationDetail(totalCount, currentPageNumber, nearest));
        }

        return Result<PaginationDetail>.Success(
            new PaginationDetail(totalCount, currentPageNumber, paginationCount));
    }

    public PaginationInfo? GetPreviousPage()
    {
        if (!HasPreviousPage)
        {
            return null;
        }

        return new PaginationInfo(CurrentPageNumber - 1, PaginationCount);
    }

    public PaginationInfo? GetNextPage()
    {
        if (!HasNextPage)
        {
            return null;
        }

        return new PaginationInfo(CurrentPageNumber + 1, PaginationCount);
    }

    public PaginationInfo? GetFirstPage() => GetPage(1);

    public PaginationInfo? GetLastPage() => GetPage(TotalPages);

    public PaginationInfo? GetPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > TotalPages)
        {
            return null;
        }

        return new PaginationInfo(pageNumber, PaginationCount);
    }
}

public class PaginationDetail<TItem> : PaginationDetail
{
    public TItem PaginatedItem { get; init; }

    public PaginationDetail(int totalCount, PaginationInfo paginationInfo, TItem paginatedItem) :
        this(totalCount, paginationInfo.PageNumber, paginationInfo.ItemCount, paginatedItem)
    {
    }

    public PaginationDetail(int totalCount, int currentPageNumber, PaginationItemCount paginationCount,
        TItem paginatedItem) :
        base(totalCount, currentPageNumber, paginationCount)
    {
        PaginatedItem = paginatedItem;
    }

    public static Result<PaginationDetail<TItem>> Create(int totalCount, PaginationInfo paginationInfo,
        TItem paginatedItem, bool adjustPaginationCount)
    {
        return Create(totalCount, paginationInfo.PageNumber, paginationInfo.ItemCount, paginatedItem,
            adjustPaginationCount);
    }

    public static Result<PaginationDetail<TItem>> Create(int totalCount, int currentPageNumber,
        PaginationItemCount paginationCount, TItem paginatedItem, bool adjustPaginationCount)
    {
        if (paginatedItem is null)
        {
            return DomainErrors.GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<TItem>>();
        }

        if (totalCount < 0)
        {
            return DomainErrors.GeneralErrors.InputIsNegative.ToErrorResult<PaginationDetail<TItem>>();
        }

        if (currentPageNumber <= 0)
        {
            return DomainErrors.GeneralErrors.InputIsNegativeOrZero.ToErrorResult<PaginationDetail<TItem>>();
        }

        if (!PaginationItemCountExtensions.IsDefined(paginationCount) && !adjustPaginationCount)
        {
            return PaginationInfoErrors.InvalidItemCount.ToErrorResult<PaginationDetail<TItem>>();
        }

        if (!PaginationItemCountExtensions.IsDefined(paginationCount) && adjustPaginationCount)
        {
            var nearest = PaginationItemCountExtensions.GetAll()
                .MinBy(x => Math.Abs((int)x - (int)paginationCount));

            return Result<PaginationDetail<TItem>>.Success(
                new PaginationDetail<TItem>(totalCount, currentPageNumber, nearest, paginatedItem));
        }

        return Result<PaginationDetail<TItem>>.Success(
            new PaginationDetail<TItem>(totalCount, currentPageNumber, paginationCount, paginatedItem));
    }
}