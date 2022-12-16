using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series.GetSeriesByIdWithBooks;

public sealed record GetSeriesByIdWithBooksQuery(SeriesId SeriesId, SortOrder SortOrder) : IQuery<DomainSeries>;