#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class BookAuthorConnector
{
    public BookId BookId { get; set; }
    public AuthorId AuthorId { get; set; }

    public Book Book { get; set; }
    public Author Author { get; set; }

    public BookAuthorConnector()
    {
    }

    public BookAuthorConnector(BookId bookId, AuthorId authorId)
    {
        BookId = bookId;
        AuthorId = authorId;
    }
}