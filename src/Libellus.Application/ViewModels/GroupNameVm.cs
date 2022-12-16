using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.ViewModels;

public sealed class GroupNameVm
{
    public GroupId GroupId { get; init; }
    public GroupFriendlyId GroupFriendlyId { get; init; }
    public Name Name { get; init; }

    public GroupNameVm(GroupId groupId, GroupFriendlyId groupFriendlyId, Name name)
    {
        GroupId = groupId;
        GroupFriendlyId = groupFriendlyId;
        Name = Guard.Against.Null(name);
    }
}