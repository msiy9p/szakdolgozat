#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Series : BaseStampedModel<SeriesId, GroupId>, ISearchable
{
    public string FriendlyId { get; set; }
    public UserId? CreatorId { get; set; }
    public string Title { get; set; }
    public string TitleNormalized { get; set; }

    public List<BookSeriesConnector> BookSeriesConnectors { get; set; } = new List<BookSeriesConnector>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Series()
    {
    }

    public Series(SeriesId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        string friendlyId, UserId? creatorId, string title, string titleNormalized) : base(id, groupId, createdOnUtc,
        modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        Title = title;
        TitleNormalized = titleNormalized;
    }
}