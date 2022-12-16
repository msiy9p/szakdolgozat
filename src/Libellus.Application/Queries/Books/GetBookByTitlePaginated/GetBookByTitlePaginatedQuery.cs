using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Books.GetBookByTitlePaginated;

public sealed record GetBookByTitlePaginatedQuery(Title Title, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Book>>>;