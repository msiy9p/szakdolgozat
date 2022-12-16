using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainBook = Libellus.Domain.Entities.Book;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using DomainShelf = Libellus.Domain.Entities.Shelf;
using PersistenceShelf = Libellus.Infrastructure.Persistence.DataModels.Shelf;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class ShelfRepository : BaseGroupedRepository<ShelfRepository, PersistenceShelf>, IShelfRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public ShelfRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<ShelfRepository> logger, IDateTimeProvider dateTimeProvider) :
        base(context, currentGroupService, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override IQueryable<PersistenceShelf> GetFiltered()
    {
        return Context.Shelves
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(ShelfId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainShelf>> GetByIdAsync(ShelfId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return ShelfErrors.ShelfNotFound.ToErrorResult<DomainShelf>();
        }

        UserVm? userVm = null;
        if (found.CreatorId.HasValue)
        {
            userVm = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        var mapResult = ShelfMapper.Map(found, userVm);
        if (mapResult.IsError)
        {
            return mapResult;
        }

        var offsetCount = await GetBookCountAsync(id, cancellationToken);
        if (offsetCount.IsSuccess)
        {
            mapResult.Value.SetBookCountOffset(offsetCount.Value);
        }

        return mapResult;
    }

    public async Task<Result<int>> GetBookCountAsync(ShelfId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return ShelfErrors.ShelfNotFound.ToErrorResult<int>();
        }

        var count = await Context.ShelfBookConnectors
            .Where(x => x.ShelfId == id)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<UserId?>> GetCreatorIdAsync(ShelfId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        if (!found)
        {
            return ShelfErrors.ShelfNotFound.ToErrorResult<UserId?>();
        }

        var value = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefaultAsync(cancellationToken);

        return value.ToResult();
    }

    public async Task<Result<DomainShelf>> GetByIdWithBooksAsync(ShelfId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return ShelfErrors.ShelfNotFound.ToErrorResult<DomainShelf>();
        }

        var persistenceBooks = await Context.Books
            .Include(x => x.ShelfBookConnectors)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.ShelfBookConnectors.Any(y => y.ShelfId == id))
            .ApplySortOrder(sortOrder)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository =
            new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        var books = new List<DomainBook>(persistenceBooks.Count);
        foreach (var book in persistenceBooks)
        {
            var temp = await bookRepository.GetByIdAsync(book, cancellationToken);
            if (temp.IsError)
            {
                return Result<DomainShelf>.Error(temp.Errors);
            }

            books.Add(temp.Value);
        }

        UserVm? userVm1 = null;
        if (found.CreatorId.HasValue)
        {
            userVm1 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        return ShelfMapper.Map(found, userVm1, books);
    }

    public async Task<Result<PaginationDetail<DomainShelf>>> GetByIdWithBooksAsync(ShelfId id,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return ShelfErrors.ShelfNotFound.ToErrorResult<PaginationDetail<DomainShelf>>();
        }

        var count = await Context.Books
            .Include(x => x.ShelfBookConnectors)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.ShelfBookConnectors.Any(y => y.ShelfId == id))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var persistenceBooks = await Context.Books
            .Include(x => x.ShelfBookConnectors)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.ShelfBookConnectors.Any(y => y.ShelfId == id))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository =
            new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        var books = new List<DomainBook>(persistenceBooks.Count);
        foreach (var book in persistenceBooks)
        {
            var temp = await bookRepository.GetByIdAsync(book, cancellationToken);
            if (temp.IsError)
            {
                return Result<PaginationDetail<DomainShelf>>.Error(temp.Errors);
            }

            books.Add(temp.Value);
        }

        UserVm? userVm1 = null;
        if (found.CreatorId.HasValue)
        {
            userVm1 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        var outputMap = ShelfMapper.Map(found, userVm1, books);
        if (outputMap.IsError)
        {
            return Result<PaginationDetail<DomainShelf>>.Error(outputMap.Errors);
        }

        outputMap.Value.SetBookCountOffset(count - persistenceBooks.Count);

        return Result<PaginationDetail<DomainShelf>>.Success(
            new PaginationDetail<DomainShelf>(count, adjusted, outputMap.Value!));
    }

    public async Task<Result<ICollection<DomainShelf>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainShelf>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = ShelfMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offsetCount = await GetBookCountAsync(item.Id, cancellationToken);
                if (offsetCount.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offsetCount.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<PaginationDetail<ICollection<DomainShelf>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainShelf>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = ShelfMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offsetCount = await GetBookCountAsync(item.Id, cancellationToken);
                if (offsetCount.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offsetCount.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainShelf>>>.Success(
            new PaginationDetail<ICollection<DomainShelf>>(count, adjusted, output));
    }

    public async Task<Result<DomainShelf>> FindByNameAsync(Name name, CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<DomainShelf>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized.Equals(name.ValueNormalized), cancellationToken);

        if (found is null)
        {
            return ShelfErrors.ShelfNotFound.ToErrorResult<DomainShelf>();
        }

        UserVm? userVm = null;
        if (found.CreatorId.HasValue)
        {
            userVm = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        var mapResult = ShelfMapper.Map(found, userVm);
        if (mapResult.IsError)
        {
            return mapResult;
        }

        var offsetCount = await GetBookCountAsync(found.Id, cancellationToken);
        if (offsetCount.IsSuccess)
        {
            mapResult.Value.SetBookCountOffset(offsetCount.Value);
        }

        return mapResult;
    }

    public async Task<Result<ICollection<DomainShelf>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainShelf>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainShelf>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = ShelfMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offsetCount = await GetBookCountAsync(item.Id, cancellationToken);
                if (offsetCount.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offsetCount.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainShelf>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainShelf>>>();
        }

        var count = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainShelf>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = ShelfMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offsetCount = await GetBookCountAsync(item.Id, cancellationToken);
                if (offsetCount.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offsetCount.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainShelf>>>.Success(
            new PaginationDetail<ICollection<DomainShelf>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainShelf>>> GetShelvesByBookIdAsync(BookId bookId, bool containing,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var exists = await Context.Books
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == bookId, cancellationToken);

        if (!exists)
        {
            return BookErrors.BookNotFound.ToErrorResult<ICollection<DomainShelf>>();
        }

        List<PersistenceShelf> shelves = new();
        if (containing)
        {
            var temp = await Context.ShelfBookConnectors
                .Include(x => x.Shelf)
                .Where(x => x.Shelf.GroupId == CurrentGroupId)
                .Where(x => x.BookId == bookId)
                .Select(x => x.Shelf)
                .ApplySortOrder(sortOrder)
                .ToListAsync(cancellationToken);

            shelves = temp;
        }
        else
        {
            var temp = await Context.Shelves
                .Include(x => x.ShelfBookConnectors)
                .Where(x => x.GroupId == CurrentGroupId)
                .Where(x => !x.ShelfBookConnectors.Any() || x.ShelfBookConnectors.All(y => y.BookId != bookId))
                .ApplySortOrder(sortOrder)
                .ToListAsync(cancellationToken);

            shelves = temp;
        }

        var output = new List<DomainShelf>(shelves.Count);
        foreach (var item in shelves)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = ShelfMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offsetCount = await GetBookCountAsync(item.Id, cancellationToken);
                if (offsetCount.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offsetCount.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ShelfId>> AddIfNotExistsAsync(DomainShelf entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<ShelfId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return found.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = ShelfMapper.Map(entity, CurrentGroupId);

        await Context.Shelves.AddAsync(item, cancellationToken);

        var changeTracker = entity.GetBookTracker();
        if (!changeTracker.HasChanges)
        {
            return entity.Id.ToResult();
        }

        foreach (var bookId in changeTracker.GetNewItemsChronologically())
        {
            var foundCon = await Context.ShelfBookConnectors
                .Where(x => x.ShelfId == item.Id)
                .AnyAsync(x => x.BookId == bookId, cancellationToken);

            if (!foundCon)
            {
                var sbc = new ShelfBookConnector(item.Id, bookId);
                await Context.ShelfBookConnectors.AddAsync(sbc, cancellationToken);
            }
        }

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainShelf entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return ShelfErrors.ShelfAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = ShelfMapper.Map(entity, CurrentGroupId);

        Context.Shelves.Update(item);

        var changeTracker = entity.GetBookTracker();
        if (!changeTracker.HasChanges)
        {
            return Result.Success();
        }


        await Context.ShelfBookConnectors
            .Where(x => x.ShelfId == item.Id)
            .Where(x => changeTracker.GetRemovedIds().Contains(x.BookId))
            .ExecuteDeleteAsync(cancellationToken);

        foreach (var bookId in changeTracker.GetNewIds())
        {
            var foundCon = await Context.ShelfBookConnectors
                .Where(x => x.ShelfId == item.Id)
                .AnyAsync(x => x.BookId == bookId, cancellationToken);

            if (!foundCon)
            {
                var sbc = new ShelfBookConnector(item.Id, bookId);
                await Context.ShelfBookConnectors.AddAsync(sbc, cancellationToken);
            }
        }

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(ShelfId id, CancellationToken cancellationToken = default)
    {
        await Context.Shelves.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainShelf entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Shelves.Remove(ShelfMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class ShelfRepositoryHelper
{
    public static IQueryable<PersistenceShelf> ApplySortOrder(this IQueryable<PersistenceShelf> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.NameNormalized)
                    .ThenBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.NameNormalized)
                    .ThenByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceShelf> ApplyPagination(this IQueryable<PersistenceShelf> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}