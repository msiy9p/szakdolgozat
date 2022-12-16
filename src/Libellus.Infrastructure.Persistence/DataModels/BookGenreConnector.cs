#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class BookGenreConnector
{
    public BookId BookId { get; set; }
    public GenreId GenreId { get; set; }

    public Book Book { get; set; }
    public Genre Genre { get; set; }

    public BookGenreConnector()
    {
    }

    public BookGenreConnector(BookId bookId, GenreId genreId)
    {
        BookId = bookId;
        GenreId = genreId;
    }
}