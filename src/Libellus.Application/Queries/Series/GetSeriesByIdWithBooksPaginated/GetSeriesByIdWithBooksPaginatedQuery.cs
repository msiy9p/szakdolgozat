using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series.GetSeriesByIdWithBooksPaginated;

public sealed record GetSeriesByIdWithBooksPaginatedQuery(SeriesId SeriesId, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<DomainSeries>>;