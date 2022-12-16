using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities.Identity;
using Libellus.Domain.Enums;
using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class GroupMembership : BaseEntity<GroupId>
{
    private readonly ChangeTracker<UserId> _userIdTracker = new();
    private readonly HashSet<UserId> _userIdUpdates = new();
    private readonly Dictionary<UserId, GroupMembershipItem> _membershipItems = new();

    public int Count => _membershipItems.Count;
    public bool HasChanges => _userIdTracker.HasChanges || _userIdUpdates.Count > 0;

    private GroupMembership(GroupId id) : base(id)
    {
    }

    public new static Result<GroupMembership> Create(GroupId id)
    {
        var result = BaseEntity<GroupId>.Create(id);
        if (result.IsError)
        {
            return Result<GroupMembership>.Invalid(result.Errors);
        }

        return new GroupMembership(id).ToResult();
    }

    public static Result<GroupMembership> Create(GroupId id, IEnumerable<GroupMembershipItem> items)
    {
        var result = BaseEntity<GroupId>.Create(id);
        if (result.IsError)
        {
            return Result<GroupMembership>.Invalid(result.Errors);
        }

        if (items is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<GroupMembership>();
        }

        var output = new GroupMembership(id);

        foreach (var item in items)
        {
            output._membershipItems.Add(item.UserId, item);
        }

        return output.ToResult();
    }

    public IReadOnlySet<UserId> GetRemovedItems() => _userIdTracker.GetRemovedItems();

    public IReadOnlySet<GroupMembershipItem> GetItems() => _membershipItems.Values.ToHashSet();

    public IReadOnlyCollection<GroupMembershipItem> GetNewItems()
    {
        var newIds = _userIdTracker.GetNewItems();

        if (newIds.Count < 1)
        {
            return Array.Empty<GroupMembershipItem>();
        }

        var output = new List<GroupMembershipItem>(newIds.Count);
        foreach (var item in _membershipItems)
        {
            if (newIds.Contains(item.Key))
            {
                output.Add(item.Value);
            }
        }

        return output;
    }

    public IReadOnlyCollection<GroupMembershipItem> GetUpdatedItems()
    {
        if (_userIdUpdates.Count < 1)
        {
            return Array.Empty<GroupMembershipItem>();
        }

        var output = new List<GroupMembershipItem>(_userIdUpdates.Count);
        foreach (var item in _userIdUpdates)
        {
            output.Add(_membershipItems[item]);
        }

        return output;
    }

    public bool Add(User user, GroupRole groupRole, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (user is null)
        {
            return false;
        }

        return Add(user.Id, groupRole, dateTimeProvider);
    }

    public bool Add(UserId userId, GroupRole groupRole, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (_membershipItems.ContainsKey(userId))
        {
            return false;
        }

        if (!GroupRoleExtensions.IsDefined(groupRole))
        {
            return false;
        }

        var datetime = dateTimeProvider.UtcNow;
        var membership = new GroupMembershipItem(userId, GroupRole.Member, datetime, datetime);
        _membershipItems.Add(userId, membership);

        _userIdTracker.Add(userId);

        return true;
    }

    public bool Update(User user, GroupRole groupRole, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (user is null)
        {
            return false;
        }

        return Update(user.Id, groupRole, dateTimeProvider);
    }

    public bool Update(UserId userId, GroupRole groupRole, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_membershipItems.ContainsKey(userId))
        {
            return false;
        }

        if (!GroupRoleExtensions.IsDefined(groupRole))
        {
            return false;
        }

        var membership = _membershipItems[userId];
        var value = membership.ChangeRole(groupRole, dateTimeProvider);

        _userIdUpdates.Add(userId);

        return value;
    }

    public bool Remove(User user)
    {
        if (user is null)
        {
            return false;
        }

        return Remove(user.Id);
    }

    public bool Remove(UserId userId)
    {
        var value = _membershipItems.Remove(userId);

        _userIdTracker.Remove(userId);
        _userIdUpdates.Remove(userId);

        return value;
    }

    public sealed class GroupMembershipItem : IEquatable<GroupMembershipItem>
    {
        public UserId UserId { get; init; }
        public GroupRole GroupRole { get; private set; }
        public ZonedDateTime CreatedOnUtc { get; init; }
        public ZonedDateTime ModifiedOnUtc { get; private set; }

        public GroupMembershipItem(UserId userId, GroupRole groupRole, ZonedDateTime createdOnUtc,
            ZonedDateTime modifiedOnUtc)
        {
            UserId = userId;

            if (!GroupRoleExtensions.IsDefined(groupRole))
            {
                throw new GroupRoleInvalidException(nameof(groupRole), (int)groupRole, typeof(GroupRole));
            }

            if (!Utilities.Utilities.DoesPrecede(createdOnUtc, modifiedOnUtc))
            {
                throw new CreationModificationDateTimeException();
            }

            GroupRole = groupRole;
            CreatedOnUtc = createdOnUtc;
            ModifiedOnUtc = modifiedOnUtc;
        }

        public static Result<GroupMembershipItem> Create(UserId userId, GroupRole groupRole, ZonedDateTime createdOnUtc,
            ZonedDateTime modifiedOnUtc)
        {
            if (!GroupRoleExtensions.IsDefined(groupRole))
            {
                return GroupRoleErrors.InvalidGroupRole.ToErrorResult<GroupMembershipItem>();
            }

            if (!Utilities.Utilities.DoesPrecede(createdOnUtc, modifiedOnUtc))
            {
                return StampedEntityErrors.CreationModificationDateTimeError.ToErrorResult<GroupMembershipItem>();
            }

            return new GroupMembershipItem(userId, groupRole, createdOnUtc, modifiedOnUtc).ToResult();
        }

        public bool ChangeRole(GroupRole groupRole, IDateTimeProvider dateTimeProvider)
        {
            if (!GroupRoleExtensions.IsDefined(groupRole))
            {
                return false;
            }

            if (GroupRole != groupRole)
            {
                var datetime = dateTimeProvider.UtcNow;
                if (!Utilities.Utilities.DoesPrecede(CreatedOnUtc, datetime))
                {
                    return false;
                }

                GroupRole = groupRole;
                ModifiedOnUtc = datetime;
                return true;
            }

            return false;
        }

        public bool Equals(GroupMembershipItem? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return UserId.Equals(other.UserId);
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is GroupMembershipItem other && Equals(other);

        public override int GetHashCode() => UserId.GetHashCode();

        public static bool operator ==(GroupMembershipItem? left, GroupMembershipItem? right) => Equals(left, right);

        public static bool operator !=(GroupMembershipItem? left, GroupMembershipItem? right) => !Equals(left, right);
    }
}