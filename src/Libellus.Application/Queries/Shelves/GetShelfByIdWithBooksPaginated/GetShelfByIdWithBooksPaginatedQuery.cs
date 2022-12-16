using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Shelves.GetShelfByIdWithBooksPaginated;

public sealed record GetShelfByIdWithBooksPaginatedQuery(ShelfId ShelfId, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<Shelf>>;