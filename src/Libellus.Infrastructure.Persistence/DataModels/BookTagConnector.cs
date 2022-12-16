#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class BookTagConnector
{
    public BookId BookId { get; set; }
    public TagId TagId { get; set; }

    public Book Book { get; set; }
    public Tag Tag { get; set; }

    public BookTagConnector()
    {
    }

    public BookTagConnector(BookId bookId, TagId tagId)
    {
        BookId = bookId;
        TagId = tagId;
    }
}