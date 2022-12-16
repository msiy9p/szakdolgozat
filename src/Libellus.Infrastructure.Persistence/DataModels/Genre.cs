#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Genre : BaseStampedModel<GenreId, GroupId>, ISearchable
{
    public UserId? CreatorId { get; set; }

    public string Name { get; set; }
    public string NameNormalized { get; set; }
    public bool IsFiction { get; set; }

    public List<BookGenreConnector> BookGenreConnectors { get; set; } = new List<BookGenreConnector>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Genre()
    {
    }

    public Genre(GenreId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId? creatorId, string name, string nameNormalized, bool isFiction) : base(id, groupId, createdOnUtc,
        modifiedOnUtc)
    {
        CreatorId = creatorId;
        Name = name;
        NameNormalized = nameNormalized;
        IsFiction = isFiction;
    }
}