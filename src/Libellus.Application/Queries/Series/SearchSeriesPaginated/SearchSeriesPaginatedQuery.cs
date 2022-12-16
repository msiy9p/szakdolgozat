using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Series.SearchSeriesPaginated;

public sealed record SearchSeriesPaginatedQuery(SearchTerm SearchTerm, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Domain.Entities.Series>>>;