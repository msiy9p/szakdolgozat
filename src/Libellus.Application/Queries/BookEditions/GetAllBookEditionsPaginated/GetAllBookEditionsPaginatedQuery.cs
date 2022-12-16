using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.BookEditions.GetAllBookEditionsPaginated;

public sealed record GetAllBookEditionsPaginatedQuery
    (int PageNumber, int ItemCount, SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<BookEdition>>>;