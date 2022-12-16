using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetGroupByIdWithPostsPaginated;

[Authorise]
public sealed record GetGroupByIdWithPostsPaginatedQuery(GroupId GroupId, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<Group>>;