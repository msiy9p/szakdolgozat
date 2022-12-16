using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.BookEditions.GetAllBookEditions;

public sealed record GetAllBookEditionsQuery(SortOrder SortOrder) : IQuery<ICollection<BookEdition>>
{
    public static readonly GetAllBookEditionsQuery DefaultInstance = new(SortOrder.Ascending);
}