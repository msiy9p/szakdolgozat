using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.ValueObjects;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series.SearchSeries;

public sealed record SearchSeriesQuery(SearchTerm SearchTerm, SortOrder SortOrder) : IQuery<ICollection<DomainSeries>>;