using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series.GetSeriesById;

public sealed record GetSeriesByIdQuery(SeriesId SeriesId) : IQuery<DomainSeries>;