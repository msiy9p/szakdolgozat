#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class BookWarningTagConnector
{
    public BookId BookId { get; set; }
    public WarningTagId WarningTagId { get; set; }

    public Book Book { get; set; }
    public WarningTag WarningTag { get; set; }

    public BookWarningTagConnector()
    {
    }

    public BookWarningTagConnector(BookId bookId, WarningTagId warningTagId)
    {
        BookId = bookId;
        WarningTagId = warningTagId;
    }
}