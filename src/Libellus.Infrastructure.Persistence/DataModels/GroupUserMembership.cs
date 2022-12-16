#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class GroupUserMembership
{
    public GroupId GroupId { get; set; }
    public UserId UserId { get; set; }
    public Guid GroupRoleId { get; set; }
    public ZonedDateTime CreatedOnUtc { get; set; }
    public ZonedDateTime ModifiedOnUtc { get; set; }

    public Group Group { get; set; }
    public ApplicationUser User { get; set; }
    public GroupRole GroupRole { get; set; }

    public GroupUserMembership()
    {
    }

    public GroupUserMembership(GroupId groupId, UserId userId, Guid groupRoleId, ZonedDateTime createdOnUtc,
        ZonedDateTime modifiedOnUtc)
    {
        GroupId = groupId;
        UserId = userId;
        GroupRoleId = groupRoleId;
        CreatedOnUtc = createdOnUtc;
        ModifiedOnUtc = modifiedOnUtc;
    }
}