using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIsbnPaginated;

public sealed record GetBookEditionByIsbnPaginatedQuery(Isbn Isbn, int PageNumber, int ItemCount, SortOrder SortOrder) :
    IQuery<PaginationDetail<ICollection<BookEdition>>>;