using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.ValueObjects;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series.GetSeriesByTitle;

public sealed record GetSeriesByTitleQuery(Title Title, SortOrder SortOrder) : IQuery<ICollection<DomainSeries>>;