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
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainBook = Libellus.Domain.Entities.Book;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using DomainSeries = Libellus.Domain.Entities.Series;
using PersistenceSeries = Libellus.Infrastructure.Persistence.DataModels.Series;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class SeriesRepository : BaseGroupedRepository<SeriesRepository, PersistenceSeries>, ISeriesRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public SeriesRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<SeriesRepository> logger, IDateTimeProvider dateTimeProvider) : base(context, currentGroupService,
        logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    internal SeriesRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger,
        IDateTimeProvider dateTimeProvider) : base(context,
        currentGroupId, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override IQueryable<PersistenceSeries> GetFiltered()
    {
        return Context.Series
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(SeriesId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainSeries>> GetByIdAsync(SeriesId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return SeriesErrors.SeriesNotFound.ToErrorResult<DomainSeries>();
        }

        UserVm? userVm = null;
        if (found.CreatorId.HasValue)
        {
            userVm = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        var map = SeriesMapper.Map(found, userVm);

        var offset = await GetBookCountAsync(id, cancellationToken);
        if (offset.IsSuccess)
        {
            map.Value.SetBookCountOffset(offset.Value);
        }

        return map;
    }

    public async Task<Result<ICollection<DomainSeries>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainSeries>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = SeriesMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offset = await GetBookCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offset.Value);
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

    public async Task<Result<int>> GetBookCountAsync(SeriesId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return SeriesErrors.SeriesNotFound.ToErrorResult<int>();
        }

        var count = await Context.BookSeriesConnectors
            .Where(x => x.SeriesId == id)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public Result<UserId?> GetCreatorId(SeriesId id)
    {
        var found = GetFiltered()
            .Any(x => x.Id == id);

        if (!found)
        {
            return SeriesErrors.SeriesNotFound.ToErrorResult<UserId?>();
        }

        var value = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return value.ToResult();
    }

    public async Task<Result<DomainSeries>> GetByIdWithBooksAsync(SeriesId id,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return SeriesErrors.SeriesNotFound.ToErrorResult<DomainSeries>();
        }

        var persistenceBooks = await Context.Books
            .Include(x => x.BookSeriesConnector)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookSeriesConnector != null && x.BookSeriesConnector.SeriesId == id)
            .ApplySortOrder(sortOrder)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        UserVm? userVm1 = null;
        if (found.CreatorId.HasValue)
        {
            userVm1 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        IBookReadOnlyRepository bookRepository =
            new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        var books = new List<DomainBook>(persistenceBooks.Count);
        foreach (var book in persistenceBooks)
        {
            var temp = await bookRepository.GetByIdAsync(book, cancellationToken);
            if (temp.IsError)
            {
                return Result<DomainSeries>.Error(temp.Errors);
            }

            books.Add(temp.Value);
        }

        return SeriesMapper.Map(found, userVm1, books);
    }

    public async Task<Result<PaginationDetail<DomainSeries>>> GetByIdWithBooksAsync(SeriesId id,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return SeriesErrors.SeriesNotFound.ToErrorResult<PaginationDetail<DomainSeries>>();
        }

        var count = await Context.Books
            .Include(x => x.BookSeriesConnector)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookSeriesConnector != null && x.BookSeriesConnector.SeriesId == found.Id)
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var persistenceBooks = await Context.Books
            .Include(x => x.BookSeriesConnector)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookSeriesConnector != null && x.BookSeriesConnector.SeriesId == id)
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        UserVm? userVm2 = null;
        if (found.CreatorId.HasValue)
        {
            userVm2 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        IBookReadOnlyRepository bookRepository =
            new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        var books = new List<DomainBook>(persistenceBooks.Count);
        foreach (var book in persistenceBooks)
        {
            var temp = await bookRepository.GetByIdAsync(book, cancellationToken);
            if (temp.IsError)
            {
                return Result<PaginationDetail<DomainSeries>>.Error(temp.Errors);
            }

            books.Add(temp.Value);
        }

        var outputResult = SeriesMapper.Map(found, userVm2, books);
        if (outputResult.IsError)
        {
            return Result<PaginationDetail<DomainSeries>>.Error(outputResult.Errors);
        }

        outputResult.Value.SetBookCountOffset(count - persistenceBooks.Count);

        return Result<PaginationDetail<DomainSeries>>.Success(
            new PaginationDetail<DomainSeries>(count, adjusted, outputResult.Value!));
    }

    public async Task<Result<PaginationDetail<ICollection<DomainSeries>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainSeries>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = SeriesMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offset = await GetBookCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainSeries>>>.Success(
            new PaginationDetail<ICollection<DomainSeries>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainSeries>>> FindByTitleAsync(Title title,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainSeries>>();
        }

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainSeries>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = SeriesMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offset = await GetBookCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainSeries>>>> FindByTitleAsync(Title title,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainSeries>>>();
        }

        var count = await Context.Books
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainSeries>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = SeriesMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offset = await GetBookCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainSeries>>>.Success(
            new PaginationDetail<ICollection<DomainSeries>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainSeries>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainSeries>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainSeries>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = SeriesMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offset = await GetBookCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainSeries>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainSeries>>>();
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

        var output = new List<DomainSeries>(found.Count);
        foreach (var item in found)
        {
            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = SeriesMapper.Map(item, userVm);

            if (map.IsSuccess)
            {
                var offset = await GetBookCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainSeries>>>.Success(
            new PaginationDetail<ICollection<DomainSeries>>(count, adjusted, output));
    }

    public async Task<Result<SeriesId>> AddIfNotExistsAsync(DomainSeries entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<SeriesId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = SeriesMapper.Map(entity, CurrentGroupId);

        await Context.Series.AddAsync(item, cancellationToken);

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainSeries entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = SeriesMapper.Map(entity, CurrentGroupId);

        Context.Series.Update(item);

        var changeTracker = entity.GetBookTracker();
        if (!changeTracker.HasChanges)
        {
            return Result.Success();
        }

        await Context.BookSeriesConnectors
            .Where(x => x.SeriesId == item.Id)
            .Where(x => changeTracker.GetRemovedIds().Contains(x.BookId))
            .ExecuteDeleteAsync(cancellationToken);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(SeriesId id, CancellationToken cancellationToken = default)
    {
        await Context.Series.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainSeries entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Series.Remove(SeriesMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class SeriesRepositoryHelper
{
    public static IQueryable<PersistenceSeries> ApplySortOrder(this IQueryable<PersistenceSeries> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.TitleNormalized)
                    .ThenBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.TitleNormalized)
                    .ThenByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceSeries> ApplyPagination(this IQueryable<PersistenceSeries> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}