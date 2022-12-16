#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class BookSeriesConnector
{
    public BookId BookId { get; set; }
    public SeriesId SeriesId { get; set; }

    public decimal NumberInSeries { get; set; }

    public Book Book { get; set; }
    public Series Series { get; set; }

    public BookSeriesConnector()
    {
    }

    public BookSeriesConnector(BookId bookId, SeriesId seriesId, decimal numberInSeries)
    {
        BookId = bookId;
        SeriesId = seriesId;
        NumberInSeries = numberInSeries;
    }
}