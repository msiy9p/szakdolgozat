using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;

namespace Libellus.Application.Queries.Series.GetAllSeriesPaginated;

public sealed record GetAllSeriesPaginatedQuery(int PageNumber, int ItemCount,
    SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Domain.Entities.Series>>>;