using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Series.GetSeriesByTitlePaginated;

public sealed record GetSeriesByTitlePaginatedQuery(Title Title, int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Domain.Entities.Series>>>;