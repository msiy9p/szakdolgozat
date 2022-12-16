using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.Mapping.Interfaces;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct GroupMembershipMapper : IMapFrom<GroupId, IEnumerable<GroupUserMembership>, Result<GroupMembership>>
{
    public static Result<GroupMembership> Map(GroupId item1, IEnumerable<GroupUserMembership> item2)
    {
        return GroupMembership.Create(item1,
            item2.Select(x =>
                new GroupMembership.GroupMembershipItem(x.UserId,
                    GroupRoleExtensions.FromString(x.GroupRole.Name).Value,
                    x.CreatedOnUtc,
                    x.ModifiedOnUtc)));
    }
}