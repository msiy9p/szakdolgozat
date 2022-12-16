using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
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
using DomainNote = Libellus.Domain.Entities.Note;
using PersistenceNote = Libellus.Infrastructure.Persistence.DataModels.Note;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class NoteRepository : BaseGroupedRepository<NoteRepository, PersistenceNote>, INoteRepository
{
    public NoteRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<NoteRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal NoteRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceNote> GetFiltered()
    {
        return Context.Notes
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(NoteId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainNote>> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return NoteErrors.NoteNotFound.ToErrorResult<DomainNote>();
        }

        if (!found.CreatorId.HasValue)
        {
            return NoteMapper.Map(found, null);
        }

        var userVm = await GetUserPicturedVmAsync(found.CreatorId.Value, cancellationToken);
        if (userVm is null)
        {
            var userVm2 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
            var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

            return NoteMapper.Map(found, temp.Value!);
        }

        return NoteMapper.Map(found, userVm);
    }

    public async Task<Result<ICollection<DomainNote>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainNote>(found.Count);
        foreach (var item in found)
        {
            UserPicturedVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserPicturedVmAsync(item.CreatorId.Value, cancellationToken);
                if (userVm is null)
                {
                    var userVm2 = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
                    var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                    var map = NoteMapper.Map(item, temp.Value!);

                    if (map.IsSuccess)
                    {
                        output.Add(map.Value);
                    }
                }
                else
                {
                    var map = NoteMapper.Map(item, userVm);

                    if (map.IsSuccess)
                    {
                        output.Add(map.Value);
                    }
                }
            }
            else
            {
                var map = NoteMapper.Map(item, userVm);

                if (map.IsSuccess)
                {
                    output.Add(map.Value);
                }
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

    public async Task<Result<UserId?>> GetCreatorIdAsync(NoteId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefaultAsync(cancellationToken);

        return found.ToResult();
    }

    public Result<UserId?> GetCreatorId(NoteId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<NoteId>> AddIfNotExistsAsync(DomainNote entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<NoteId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = NoteMapper.Map(entity, CurrentGroupId);

        await Context.Notes.AddAsync(item, cancellationToken);

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainNote entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = NoteMapper.Map(entity, CurrentGroupId);

        Context.Notes.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(NoteId id, CancellationToken cancellationToken = default)
    {
        await Context.Notes.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainNote entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Notes.Remove(NoteMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class NoteRepositoryHelper
{
    public static IQueryable<PersistenceNote> ApplySortOrder(this IQueryable<PersistenceNote> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceNote> ApplyPagination(this IQueryable<PersistenceNote> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}