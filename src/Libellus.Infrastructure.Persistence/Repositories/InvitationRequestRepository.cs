using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainInvitationRequest = Libellus.Domain.Entities.InvitationRequest;
using PersistenceInvitationRequest = Libellus.Infrastructure.Persistence.DataModels.InvitationRequest;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class InvitationRequestRepository : BaseRepository<InvitationRequestRepository>,
    IInvitationRequestRepository
{
    public InvitationRequestRepository(ApplicationContext context, ILogger<InvitationRequestRepository> logger) : base(
        context,
        logger)
    {
    }

    internal InvitationRequestRepository(ApplicationContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<Result<bool>> ExistsAsync(InvitationId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.InvitationRequests
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainInvitationRequest>> GetByIdAsync(InvitationId id,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.InvitationRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return InvitationErrors.InvitationNotFound.ToErrorResult<DomainInvitationRequest>();
        }

        return InvitationRequestMapper.Map(found);
    }

    public async Task<Result<InvitationRequestVm>> GetVmByIdAsync(InvitationId id,
        CancellationToken cancellationToken = default)
    {
        InvitationRequestVm? found = null;
        try
        {
            found = await
                (from i in Context.InvitationRequests
                    where i.Id == id
                    select new InvitationRequestVm(i.Id, i.Group.Id, i.Requester.UserName))
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception)
        {
            return InvitationErrors.InvitationNotFound.ToErrorResult<InvitationRequestVm>();
        }

        if (found is null)
        {
            return InvitationErrors.InvitationNotFound.ToErrorResult<InvitationRequestVm>();
        }

        return found.ToResult();
    }

    public async Task<Result<ICollection<DomainInvitationRequest>>> GetAllAsync(
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.InvitationRequests
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitationRequest>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationRequestMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitationRequest>>> GetAllAsync(InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.InvitationRequests
            .Where(x => x.InvitationStatus == status)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitationRequest>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationRequestMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitationRequest>>> GetAllToExpireAsync(
        IDateTimeProvider dateTimeProvider,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var dateTime = dateTimeProvider.UtcNow;

        var found = await Context.InvitationRequests
            .Where(x => x.InvitationStatus == InvitationStatus.Pending)
            .Where(x => (dateTime - x.CreatedOnUtc).TotalDays > DomainInvitationRequest.ExpireAfterDays.TotalDays)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitationRequest>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationRequestMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitationRequest>>> GetAllByGroupIdAsync(GroupId groupId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.InvitationRequests
            .Where(x => x.GroupId == groupId)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitationRequest>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationRequestMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitationRequest>>> GetAllByGroupIdAsync(GroupId groupId,
        InvitationStatus status, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.InvitationRequests
            .Where(x => x.InvitationStatus == status)
            .Where(x => x.GroupId == groupId)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitationRequest>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationRequestMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result> AddIfNotExistsAsync(DomainInvitationRequest entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return exists;
        }

        if (exists.Value)
        {
            return Result.Success();
        }

        var found = await Context.InvitationRequests
            .Where(x => x.GroupId == entity.GroupId)
            .Where(x => x.InvitationStatus == InvitationStatus.Pending)
            .AnyAsync(x => x.RequesterId == entity.RequesterId, cancellationToken);

        if (found)
        {
            return Result.Success();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = InvitationRequestMapper.Map(entity);

        await Context.InvitationRequests.AddAsync(item, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(DomainInvitationRequest entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = InvitationRequestMapper.Map(entity);

        Context.InvitationRequests.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(InvitationId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistenceInvitationRequest()
        {
            Id = id
        };

        Context.InvitationRequests.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainInvitationRequest entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.InvitationRequests.Remove(InvitationRequestMapper.Map(entity));

        return await Task.FromResult(Result.Success());
    }
}

internal static class InvitationRequestRepositoryHelper
{
    public static IQueryable<PersistenceInvitationRequest> ApplySortOrder(
        this IQueryable<PersistenceInvitationRequest> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.CreatedOnUtc)
                    .ThenBy(x => x.ModifiedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.CreatedOnUtc)
                    .ThenByDescending(x => x.ModifiedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceInvitationRequest> ApplyPagination(
        this IQueryable<PersistenceInvitationRequest> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}