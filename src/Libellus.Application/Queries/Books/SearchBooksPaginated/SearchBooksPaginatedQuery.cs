using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Books.SearchBooksPaginated;

public sealed record SearchBooksPaginatedQuery(SearchTerm SearchTerm, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Book>>>;