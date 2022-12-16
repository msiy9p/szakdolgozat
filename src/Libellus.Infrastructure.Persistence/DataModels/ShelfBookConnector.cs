#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class ShelfBookConnector
{
    public ShelfId ShelfId { get; set; }
    public BookId BookId { get; set; }

    public Shelf Shelf { get; set; }
    public Book Book { get; set; }

    public ShelfBookConnector()
    {
    }

    public ShelfBookConnector(ShelfId shelfId, BookId bookId)
    {
        ShelfId = shelfId;
        BookId = bookId;
    }
}