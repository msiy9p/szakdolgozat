using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
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
using DomainInvitation = Libellus.Domain.Entities.Invitation;
using PersistenceInvitation = Libellus.Infrastructure.Persistence.DataModels.Invitation;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class InvitationRepository : BaseRepository<InvitationRepository>, IInvitationRepository
{
    public InvitationRepository(ApplicationContext context, ILogger<InvitationRepository> logger) : base(context,
        logger)
    {
    }

    internal InvitationRepository(ApplicationContext context, ILogger logger) : base(context, logger)
    {
    }

    private async Task<UserVm?> GetUserVmAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        IUserReadOnlyRepository userRepository = new UserRepository(Context, Logger);
        var result = await userRepository.GetVmByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return null;
        }

        return result.Value;
    }

    private async Task<UserPicturedVm?> GetUserPicturedVmAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        IUserReadOnlyRepository userRepository = new UserRepository(Context, Logger);
        var result = await userRepository.GetPicturedVmByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return null;
        }

        return result.Value;
    }

    public async Task<Result<bool>> ExistsAsync(InvitationId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainInvitation>> GetByIdAsync(InvitationId id,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return InvitationErrors.InvitationNotFound.ToErrorResult<DomainInvitation>();
        }

        return InvitationMapper.Map(found);
    }

    public async Task<Result<InvitationVm>> GetVmByIdAsync(InvitationId id,
        CancellationToken cancellationToken = default)
    {
        InvitationVm? found = null;
        try
        {
            found = await
                (from i in Context.Invitations
                    where i.Id == id
                    select new InvitationVm(i.Id, i.Group.Name, i.Inviter.UserName, i.Invitee.Email,
                        i.Invitee.UserName))
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception)
        {
            return InvitationErrors.InvitationNotFound.ToErrorResult<InvitationVm>();
        }

        if (found is null)
        {
            return InvitationErrors.InvitationNotFound.ToErrorResult<InvitationVm>();
        }

        return found.ToResult();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllAsync(InvitationStatus status,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Where(x => x.InvitationStatus == status)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllToExpireAsync(IDateTimeProvider dateTimeProvider,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var dateTime = dateTimeProvider.UtcNow;

        var found = await Context.Invitations
            .Where(x => x.InvitationStatus == InvitationStatus.Pending)
            .Where(x => (dateTime - x.CreatedOnUtc).TotalDays > DomainInvitation.ExpireAfterDays.TotalDays)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllByInviteeIdAsync(UserId inviteeId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Where(x => x.InviteeId == inviteeId)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllByInviteeIdAsync(UserId inviteeId,
        InvitationStatus status, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Where(x => x.InvitationStatus == status)
            .Where(x => x.InviteeId == inviteeId)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllByGroupIdAsync(GroupId groupId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Where(x => x.GroupId == groupId)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainInvitation>>> GetAllByGroupIdAsync(GroupId groupId,
        InvitationStatus status, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Where(x => x.InvitationStatus == status)
            .Where(x => x.GroupId == groupId)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainInvitation>(found.Count);
        foreach (var item in found)
        {
            var map = InvitationMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<InvitationPicturedVm>>> GetAllPicturedVmByGroupIdAsync(GroupId groupId,
        InvitationStatus status, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Where(x => x.InvitationStatus == status)
            .Where(x => x.GroupId == groupId)
            .ApplySortOrder(sortOrder)
            .Select(x => new { x.Id, x.InviteeId, x.InvitationStatus })
            .ToListAsync(cancellationToken);

        var output = new List<InvitationPicturedVm>(found.Count);
        foreach (var item in found)
        {
            var userVm = await GetUserPicturedVmAsync(item.InviteeId, cancellationToken);
            if (userVm is null)
            {
                var userVm2 = await GetUserVmAsync(item.InviteeId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                output.Add(new InvitationPicturedVm(item.Id, temp.Value, item.InvitationStatus));
            }
            else
            {
                output.Add(new InvitationPicturedVm(item.Id, userVm, item.InvitationStatus));
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<InvitationUserVm>>> GetAllVmByInviteeIdAsync(UserId inviteeId,
        InvitationStatus status, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Invitations
            .Include(x => x.Group)
            .Where(x => x.InvitationStatus == status)
            .Where(x => x.InviteeId == inviteeId)
            .ApplySortOrder(sortOrder)
            .Select(x => new { x.Id, x.Group.Name, x.InvitationStatus })
            .ToListAsync(cancellationToken);

        var output = new List<InvitationUserVm>(found.Count);
        foreach (var item in found)
        {
            output.Add(new InvitationUserVm(item.Id, new Name(item.Name), item.InvitationStatus));
        }

        return output.ToResultCollection();
    }

    public async Task<Result> AddIfNotExistsAsync(DomainInvitation entity,
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

        var found = await Context.Invitations
            .Where(x => x.GroupId == entity.GroupId)
            .Where(x => x.InvitationStatus == InvitationStatus.Pending)
            .AnyAsync(x => x.InviteeId == entity.InviteeId, cancellationToken);

        if (found)
        {
            return Result.Success();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = InvitationMapper.Map(entity);

        await Context.Invitations.AddAsync(item, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(DomainInvitation entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = InvitationMapper.Map(entity);

        Context.Invitations.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(InvitationId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistenceInvitation()
        {
            Id = id
        };

        Context.Invitations.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainInvitation entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Invitations.Remove(InvitationMapper.Map(entity));

        return await Task.FromResult(Result.Success());
    }
}

internal static class InvitationRepositoryHelper
{
    public static IQueryable<PersistenceInvitation> ApplySortOrder(this IQueryable<PersistenceInvitation> queryable,
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

    public static IQueryable<PersistenceInvitation> ApplyPagination(this IQueryable<PersistenceInvitation> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}