using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public sealed class SeriesDto
{
    public Title Title { get; init; }
    public decimal NumberInSeries { get; init; }
    public SeriesId? SeriesId { get; init; }

    public SeriesDto(Title title, decimal numberInSeries, SeriesId? seriesId)
    {
        Title = title;
        NumberInSeries = numberInSeries;
        SeriesId = seriesId;
    }
}