using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Groups.GetAllMemberGroupsPaginated;

[Authorise]
public sealed record GetAllMemberGroupsPaginatedQuery(int PageNumber, int ItemCount, SortOrder SortOrder) :
    IQuery<PaginationDetail<ICollection<Group>>>;