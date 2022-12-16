using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainReading = Libellus.Domain.Entities.Reading;
using PersistenceBookEdition = Libellus.Infrastructure.Persistence.DataModels.BookEdition;
using PersistenceLiteratureForm = Libellus.Infrastructure.Persistence.DataModels.LiteratureForm;
using PersistenceReading = Libellus.Infrastructure.Persistence.DataModels.Reading;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class ReadingRepository : BaseGroupedRepository<ReadingRepository, PersistenceReading>,
    IReadingRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReadingRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<ReadingRepository> logger, IDateTimeProvider dateTimeProvider) : base(context, currentGroupService,
        logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    internal ReadingRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger,
        IDateTimeProvider dateTimeProvider) : base(context,
        currentGroupId, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override IQueryable<PersistenceReading> GetFiltered()
    {
        return Context.Readings
            .Include(x => x.Note)
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(ReadingId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainReading>> GetByIdAsync(ReadingId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.GroupId == CurrentGroupId)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return ReadingErrors.ReadingNotFound.ToErrorResult<DomainReading>();
        }

        IBookEditionReadOnlyRepository bookEditionRepository =
            new BookEditionRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        var compactVm = await bookEditionRepository.GetCompactVmByIdAsync(found.BookEditionId, cancellationToken);
        if (compactVm.IsError)
        {
            return Result<DomainReading>.Error(compactVm.Errors);
        }

        var pageCount = await Context.BookEditions
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.Id == found.BookEditionId)
            .Select(x => x.PageCount)
            .FirstOrDefaultAsync(cancellationToken);

        var scoreMultiplier = await Context.BookEditions
            .Include(x => x.Book)
            .ThenInclude(x => x.LiteratureForm)
            .Where(x => x.GroupId == CurrentGroupId)
            .Select<PersistenceBookEdition, decimal?>(x =>
                x.Book.LiteratureForm == null ? null : x.Book.LiteratureForm.ScoreMultiplier)
            .FirstOrDefaultAsync(cancellationToken);

        var pbe = new PersistenceBookEdition() { Id = found.BookEditionId, PageCount = pageCount };
        var plf = scoreMultiplier.HasValue
            ? new PersistenceLiteratureForm() { ScoreMultiplier = scoreMultiplier.Value }
            : null;

        var creatorUserVm = await GetUserPicturedVmAsync(found.CreatorId, cancellationToken);
        if (creatorUserVm is null)
        {
            var userVm2 = await GetUserVmAsync(found.CreatorId, cancellationToken);
            var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
            creatorUserVm = temp.IsSuccess ? temp.Value : null;
        }

        UserPicturedVm? noteUserVm = null;
        if (found.Note?.CreatorId is not null)
        {
            noteUserVm = await GetUserPicturedVmAsync(found.Note.CreatorId.Value, cancellationToken);

            if (noteUserVm is null)
            {
                var userVm2 = await GetUserVmAsync(found.Note.CreatorId.Value, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                noteUserVm = temp.IsSuccess ? temp.Value : null;
            }
        }

        return ReadingMapper.Map(found, creatorUserVm!, noteUserVm, pbe, plf, compactVm.Value);
    }

    public async Task<Result<ICollection<DomainReading>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        IBookEditionReadOnlyRepository bookEditionRepository =
            new BookEditionRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainReading>(found.Count);
        foreach (var item in found)
        {
            var compactVm = await bookEditionRepository.GetCompactVmByIdAsync(item.BookEditionId, cancellationToken);
            if (compactVm.IsError)
            {
                return Result<ICollection<DomainReading>>.Error(compactVm.Errors);
            }

            var pageCountTask = await Context.BookEditions
                .Where(x => x.GroupId == CurrentGroupId)
                .Where(x => x.Id == item.BookEditionId)
                .Select(x => x.PageCount)
                .FirstOrDefaultAsync(cancellationToken);

            var scoreMultiplierTask = await Context.BookEditions
                .Include(x => x.Book)
                .ThenInclude(x => x.LiteratureForm)
                .Where(x => x.GroupId == CurrentGroupId)
                .Select<PersistenceBookEdition, decimal?>(x =>
                    x.Book.LiteratureForm == null ? null : x.Book.LiteratureForm.ScoreMultiplier)
                .FirstOrDefaultAsync(cancellationToken);

            var pbe = new PersistenceBookEdition() { Id = item.BookEditionId, PageCount = pageCountTask };
            var plf = scoreMultiplierTask.HasValue
                ? new PersistenceLiteratureForm() { ScoreMultiplier = scoreMultiplierTask.Value }
                : null;

            var creatorUserVm = await GetUserPicturedVmAsync(item.CreatorId, cancellationToken);
            if (creatorUserVm is null)
            {
                var userVm2 = await GetUserVmAsync(item.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                creatorUserVm = temp.IsSuccess ? temp.Value : null;
            }

            UserPicturedVm? noteUserVm = null;
            if (item.Note?.CreatorId is not null)
            {
                noteUserVm = await GetUserPicturedVmAsync(item.Note.CreatorId.Value, cancellationToken);

                if (noteUserVm is null)
                {
                    var userVm2 = await GetUserVmAsync(item.Note.CreatorId.Value, cancellationToken);
                    var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                    noteUserVm = temp.IsSuccess ? temp.Value : null;
                }
            }

            var map = ReadingMapper.Map(item, creatorUserVm!, noteUserVm, pbe, plf, compactVm.Value);

            if (map.IsSuccess)
            {
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

    public async Task<Result<UserId?>> GetCreatorIdAsync(ReadingId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return Result<UserId?>.Success(null);
        }

        UserId? temp = found;

        return temp.ToResult();
    }

    public Result<UserId?> GetCreatorId(ReadingId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        if (found == default)
        {
            return Result<UserId?>.Success(null);
        }

        UserId? temp = found;

        return temp.ToResult();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainReading>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        IBookEditionReadOnlyRepository bookEditionRepository =
            new BookEditionRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainReading>(found.Count);
        foreach (var item in found)
        {
            var compactVm = await bookEditionRepository.GetCompactVmByIdAsync(item.BookEditionId, cancellationToken);
            if (compactVm.IsError)
            {
                return Result<PaginationDetail<ICollection<DomainReading>>>.Error(compactVm.Errors);
            }

            var pageCountTask = await Context.BookEditions
                .Where(x => x.GroupId == CurrentGroupId)
                .Where(x => x.Id == item.BookEditionId)
                .Select(x => x.PageCount)
                .FirstOrDefaultAsync(cancellationToken);

            var scoreMultiplierTask = await Context.BookEditions
                .Include(x => x.Book)
                .ThenInclude(x => x.LiteratureForm)
                .Where(x => x.GroupId == CurrentGroupId)
                .Select<PersistenceBookEdition, decimal?>(x =>
                    x.Book.LiteratureForm == null ? null : x.Book.LiteratureForm.ScoreMultiplier)
                .FirstOrDefaultAsync(cancellationToken);

            var pbe = new PersistenceBookEdition() { Id = item.BookEditionId, PageCount = pageCountTask };
            var plf = scoreMultiplierTask.HasValue
                ? new PersistenceLiteratureForm() { ScoreMultiplier = scoreMultiplierTask.Value }
                : null;

            var creatorUserVm = await GetUserPicturedVmAsync(item.CreatorId, cancellationToken);
            if (creatorUserVm is null)
            {
                var userVm2 = await GetUserVmAsync(item.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                creatorUserVm = temp.IsSuccess ? temp.Value : null;
            }

            UserPicturedVm? noteUserVm = null;
            if (item.Note?.CreatorId is not null)
            {
                noteUserVm = await GetUserPicturedVmAsync(item.Note.CreatorId.Value, cancellationToken);

                if (noteUserVm is null)
                {
                    var userVm2 = await GetUserVmAsync(item.Note.CreatorId.Value, cancellationToken);
                    var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                    noteUserVm = temp.IsSuccess ? temp.Value : null;
                }
            }

            var map = ReadingMapper.Map(item, creatorUserVm!, noteUserVm, pbe, plf, compactVm.Value);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainReading>>>.Success(
            new PaginationDetail<ICollection<DomainReading>>(count, adjusted, output));
    }

    public async Task<Result<ReadingId>> AddIfNotExistsAsync(DomainReading entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<ReadingId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = ReadingMapper.Map(entity, CurrentGroupId);

        if (item.NoteId.HasValue)
        {
            INoteRepository noteRepository = new NoteRepository(Context, CurrentGroupId, Logger);
            var existResult = await noteRepository.ExistsAsync(item.NoteId.Value, cancellationToken);
            if (existResult.IsError)
            {
                return Result<ReadingId>.Error(existResult.Errors);
            }

            if (!existResult.Value)
            {
                var mapResult = await noteRepository.AddIfNotExistsAsync(entity.Note!, cancellationToken);
                if (mapResult.IsError)
                {
                    return Result<ReadingId>.Error(mapResult.Errors);
                }
            }
        }

        await Context.Readings.AddAsync(item, cancellationToken);

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainReading entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }


        if (entity.Note is not null)
        {
            INoteRepository noteRepository = new NoteRepository(Context, CurrentGroupId, Logger);

            var existResult = await noteRepository.ExistsAsync(entity.Note.Id, cancellationToken);
            if (existResult.IsError)
            {
                return existResult;
            }

            if (!existResult.Value)
            {
                var mapResult = await noteRepository.AddIfNotExistsAsync(entity.Note!, cancellationToken);
                if (mapResult.IsError)
                {
                    return mapResult;
                }
            }
            else
            {
                var mapResult = await noteRepository.UpdateAsync(entity.Note!, cancellationToken);
                if (mapResult.IsError)
                {
                    return mapResult;
                }
            }
        }

        var item = ReadingMapper.Map(entity, CurrentGroupId);

        Context.Readings.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(ReadingId id, CancellationToken cancellationToken = default)
    {
        await Context.Readings.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainReading entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Readings.Remove(ReadingMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class ReadingRepositoryHelper
{
    public static IQueryable<PersistenceReading> ApplySortOrder(this IQueryable<PersistenceReading> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.StartedOnUtc)
                    .ThenBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.StartedOnUtc)
                    .ThenByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceReading> ApplyPagination(this IQueryable<PersistenceReading> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}