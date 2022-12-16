using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using DomainSeries = Libellus.Domain.Entities.Series;

namespace Libellus.Application.Queries.Series.GetAllSeries;

public sealed record GetAllSeriesQuery(SortOrder SortOrder) : IQuery<ICollection<DomainSeries>>
{
    public static readonly GetAllSeriesQuery DefaultInstance = new(SortOrder.Ascending);
}