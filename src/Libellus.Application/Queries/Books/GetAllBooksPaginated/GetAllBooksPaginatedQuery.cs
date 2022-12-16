using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Books.GetAllBooksPaginated;

public sealed record GetAllBooksPaginatedQuery(int PageNumber, int ItemCount, SortOrder SortOrder) :
    IQuery<PaginationDetail<ICollection<Book>>>;