using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Authors.SearchAuthorsPaginated;

public sealed record SearchAuthorsPaginatedQuery(SearchTerm SearchTerm, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Author>>>;