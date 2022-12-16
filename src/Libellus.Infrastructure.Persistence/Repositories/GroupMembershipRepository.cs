using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Libellus.Infrastructure.Persistence.DataModels;
using static Libellus.Domain.Errors.DomainErrors;
using DomainGroup = Libellus.Domain.Entities.Group;
using GroupRole = Libellus.Domain.Enums.GroupRole;
using PersistenceGroup = Libellus.Infrastructure.Persistence.DataModels.Group;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class GroupMembershipRepository : BaseRepository<GroupMembershipRepository>, IGroupMembershipRepository
{
    private readonly UserId _currentUserId;

    public GroupMembershipRepository(ApplicationContext context, ICurrentUserService currentUserService,
        ILogger<GroupMembershipRepository> logger) : base(context, logger)
    {
        if (currentUserService.UserId is null)
        {
            throw new ArgumentNullException(nameof(currentUserService), "UserId cannot be null.");
        }

        _currentUserId = currentUserService.UserId.Value;
    }

    internal GroupMembershipRepository(ApplicationContext context, UserId userId, ILogger logger) : base(context,
        logger)
    {
        _currentUserId = userId;
    }

    public async Task<Result<bool>> ExistsAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .AnyAsync(x => x.Id == id, cancellationToken);

        return found.ToResult();
    }

    public async Task<Result<int>> MemberCountAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .AnyAsync(x => x.Id == id, cancellationToken);

        if (!found)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<int>();
        }

        var count = await Context.GroupUserMemberships
            .Where(x => x.GroupId == id)
            .CountAsync(cancellationToken);

        return count.ToResult();
    }

    public async Task<Result<GroupMembership>> GetByIdAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .AnyAsync(x => x.Id == id, cancellationToken);

        if (!found)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<GroupMembership>();
        }

        var memberships = await Context.GroupUserMemberships
            .Include(x => x.GroupRole)
            .Where(x => x.GroupId == id)
            .ToListAsync(cancellationToken);

        return GroupMembershipMapper.Map(id, memberships);
    }

    public async Task<Result<GroupMembership>> GetByFriendlyIdAsync(GroupFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<GroupMembership>();
        }

        var memberships = await Context.GroupUserMemberships
            .Include(x => x.GroupRole)
            .Where(x => x.GroupId == found)
            .ToListAsync(cancellationToken);

        return GroupMembershipMapper.Map(found, memberships);
    }

    public async Task<Result> UpdateAsync(GroupMembership entity, CancellationToken cancellationToken = default)
    {
        var foundResult = await ExistsAsync(entity.Id, cancellationToken);
        if (foundResult.IsError)
        {
            return foundResult;
        }

        if (!foundResult.Value)
        {
            return GroupErrors.GroupNotFound.ToErrorResult();
        }

        if (!entity.HasChanges)
        {
            return Result.Success();
        }

        var removed = entity.GetRemovedItems();
        await Context.GroupUserMemberships
            .Where(x => removed.Contains(x.UserId))
            .ExecuteDeleteAsync(cancellationToken);

        var memberId = await Context.GroupRoles
            .Where(x => x.NameNormalized == "Member".ToNormalizedUpperInvariant())
            .Select(x => x.GroupRoleId)
            .FirstAsync(cancellationToken);

        var moderatorId = await Context.GroupRoles
            .Where(x => x.NameNormalized == "Moderator".ToNormalizedUpperInvariant())
            .Select(x => x.GroupRoleId)
            .FirstAsync(cancellationToken);

        var ownerId = await Context.GroupRoles
            .Where(x => x.NameNormalized == "Owner".ToNormalizedUpperInvariant())
            .Select(x => x.GroupRoleId)
            .FirstAsync(cancellationToken);

        foreach (var membershipItem in entity.GetNewItems())
        {
            switch (membershipItem.GroupRole)
            {
                case GroupRole.Member:
                {
                    var item = new GroupUserMembership(entity.Id, membershipItem.UserId, memberId,
                        membershipItem.CreatedOnUtc, membershipItem.ModifiedOnUtc);

                    await Context.AddAsync(item, cancellationToken);
                    break;
                }
                case GroupRole.Moderator:
                {
                    var item = new GroupUserMembership(entity.Id, membershipItem.UserId, moderatorId,
                        membershipItem.CreatedOnUtc, membershipItem.ModifiedOnUtc);

                    await Context.AddAsync(item, cancellationToken);
                    break;
                }
                case GroupRole.Owner:
                {
                    var item = new GroupUserMembership(entity.Id, membershipItem.UserId, ownerId,
                        membershipItem.CreatedOnUtc, membershipItem.ModifiedOnUtc);

                    await Context.AddAsync(item, cancellationToken);
                    break;
                }
                default:
                    continue;
            }
        }

        foreach (var membershipItem in entity.GetUpdatedItems())
        {
            var oldMembership = await Context.GroupUserMemberships
                .Where(x => x.GroupId == entity.Id)
                .FirstOrDefaultAsync(x => x.UserId == membershipItem.UserId, cancellationToken);

            if (oldMembership is null)
            {
                continue;
            }

            switch (membershipItem.GroupRole)
            {
                case GroupRole.Member:
                {
                    oldMembership.GroupRoleId = memberId;

                    Context.GroupUserMemberships.Update(oldMembership);
                    break;
                }
                case GroupRole.Moderator:
                {
                    oldMembership.GroupRoleId = moderatorId;

                    Context.GroupUserMemberships.Update(oldMembership);
                    break;
                }
                case GroupRole.Owner:
                {
                    oldMembership.GroupRoleId = ownerId;

                    Context.GroupUserMemberships.Update(oldMembership);
                    break;
                }
                default:
                    continue;
            }
        }

        return Result.Success();
    }
}