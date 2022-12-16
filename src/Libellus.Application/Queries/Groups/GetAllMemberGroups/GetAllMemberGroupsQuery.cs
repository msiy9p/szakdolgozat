using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetAllMemberGroups;

[Authorise]
public sealed record GetAllMemberGroupsQuery(SortOrder SortOrder) : IQuery<ICollection<Group>>
{
    public static readonly GetAllMemberGroupsQuery DefaultInstance = new(SortOrder.Ascending);
}