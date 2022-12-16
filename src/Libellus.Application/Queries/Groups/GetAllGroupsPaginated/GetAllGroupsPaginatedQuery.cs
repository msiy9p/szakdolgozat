using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetAllGroupsPaginated;

[Authorise]
public sealed record GetAllGroupsPaginatedQuery
    (int PageNumber, int ItemCount, SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Group>>>;