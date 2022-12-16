using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.ViewModels;

namespace Libellus.Application.Queries.Groups.GetMemberGroups;

[Authorise]
public sealed record GetMemberGroupsQuery(SortOrder SortOrder) : IQuery<ICollection<GroupNameVm>>
{
    public static readonly GetMemberGroupsQuery DefaultInstance = new(SortOrder.Ascending);
}