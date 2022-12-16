using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Authors.GetAllAuthorsPaginated;

public sealed record GetAllAuthorsPaginatedQuery
    (int PageNumber, int ItemCount, SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Author>>>;