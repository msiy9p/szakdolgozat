using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Books.GetAllBooks;

public sealed record GetAllBooksQuery(SortOrder SortOrder) : IQuery<ICollection<Book>>
{
    public static readonly GetAllBooksQuery DefaultInstance = new(SortOrder.Ascending);
}