using Ardalis.GuardClauses;
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
using DomainComment = Libellus.Domain.Entities.Comment;
using PersistenceComment = Libellus.Infrastructure.Persistence.DataModels.Comment;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class CommentRepository : BaseGroupedRepository<CommentRepository, PersistenceComment>,
    ICommentRepository
{
    private readonly PostId _currentPostId;

    public CommentRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ICurrentPostService currentPostService, ILogger<CommentRepository> logger) : base(context, currentGroupService,
        logger)
    {
        _currentPostId = Guard.Against.Null(currentPostService.CurrentPostId)!.Value;
    }

    internal CommentRepository(ApplicationContext context, GroupId currentGroupId, PostId currentPostId, ILogger logger)
        : base(context, currentGroupId, logger)
    {
        _currentPostId = currentPostId;
    }

    protected override IQueryable<PersistenceComment> GetFiltered()
    {
        return Context.Comments
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.PostId == _currentPostId);
    }

    public async Task<Result<bool>> ExistsAsync(CommentId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainComment>> GetByIdAsync(CommentId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return CommentErrors.CommentNotFound.ToErrorResult<DomainComment>();
        }

        var userVm = await GetUserPicturedVmAsync(found.CreatorId, cancellationToken);
        if (userVm is null)
        {
            var userVm2 = await GetUserVmAsync(found.CreatorId, cancellationToken);
            var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

            return CommentMapper.Map(found, temp.Value!);
        }

        return CommentMapper.Map(found, userVm!);
    }

    public async Task<Result<ICollection<DomainComment>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainComment>(found.Count);
        foreach (var item in found)
        {
            var userVm = await GetUserPicturedVmAsync(item.CreatorId, cancellationToken);
            if (userVm is null)
            {
                var userVm2 = await GetUserVmAsync(item.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                var c1 = CommentMapper.Map(item, temp.Value!);
                if (c1.IsSuccess)
                {
                    output.Add(c1.Value!);
                }
            }

            var c2 = CommentMapper.Map(item, userVm!);
            if (c2.IsSuccess)
            {
                output.Add(c2.Value!);
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

    public async Task<Result<UserId?>> GetCreatorIdAsync(CommentId id, CancellationToken cancellationToken = default)
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

    public async Task<Result<PaginationDetail<ICollection<DomainComment>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainComment>(found.Count);
        foreach (var item in found)
        {
            var userVm = await GetUserPicturedVmAsync(item.CreatorId, cancellationToken);
            if (userVm is null)
            {
                var userVm2 = await GetUserVmAsync(item.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                var c1 = CommentMapper.Map(item, temp.Value!);
                if (c1.IsSuccess)
                {
                    output.Add(c1.Value!);
                }
            }

            var c2 = CommentMapper.Map(item, userVm!);
            if (c2.IsSuccess)
            {
                output.Add(c2.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainComment>>>.Success(
            new PaginationDetail<ICollection<DomainComment>>(count, adjusted, output));
    }

    public Result<UserId?> GetCreatorId(CommentId id)
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

    public async Task<Result<CommentId>> AddIfNotExistsAsync(DomainComment entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<CommentId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = CommentMapper.Map(entity, CurrentGroupId, _currentPostId);

        await Context.Comments.AddAsync(item, cancellationToken);

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainComment entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = CommentMapper.Map(entity, CurrentGroupId, _currentPostId);

        Context.Comments.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(CommentId id, CancellationToken cancellationToken = default)
    {
        await Context.Comments.Where(x => x.GroupId == CurrentGroupId && x.PostId == _currentPostId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainComment entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Comments.Remove(CommentMapper.Map(entity, CurrentGroupId, _currentPostId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class CommentRepositoryHelper
{
    public static IQueryable<PersistenceComment> ApplySortOrder(this IQueryable<PersistenceComment> queryable,
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

    public static IQueryable<PersistenceComment> ApplyPagination(this IQueryable<PersistenceComment> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}