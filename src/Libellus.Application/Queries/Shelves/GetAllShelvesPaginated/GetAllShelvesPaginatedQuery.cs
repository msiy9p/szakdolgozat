using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Shelves.GetAllShelvesPaginated;

public sealed record GetAllShelvesPaginatedQuery
    (int PageNumber, int ItemCount, SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Shelf>>>;