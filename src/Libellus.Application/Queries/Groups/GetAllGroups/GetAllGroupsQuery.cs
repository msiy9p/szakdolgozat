using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetAllGroups;

[Authorise]
public sealed record GetAllGroupsQuery(SortOrder SortOrder) : IQuery<ICollection<Group>>
{
    public static readonly GetAllGroupsQuery DefaultInstance = new(SortOrder.Ascending);
}