using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class SeriesVm
{
    public Series Series { get; init; }

    public string GroupId { get; init; }

    public string SeriesId { get; init; }

    public bool ShowEditSeries { get; init; }

    public bool ShowBackToSeries { get; init; }

    public SeriesVm(Series series, string groupId, string seriesId) : this(series, groupId, seriesId,
        showEditSeries: false, showBackToSeries: false)
    {
    }

    public SeriesVm(Series series, string groupId, string seriesId, bool showEditSeries,
        bool showBackToSeries)
    {
        Series = series;
        GroupId = groupId;
        SeriesId = seriesId;
        ShowEditSeries = showEditSeries;

        if (ShowEditSeries)
        {
            ShowBackToSeries = false;
        }
        else
        {
            ShowBackToSeries = showBackToSeries;
        }
    }
}