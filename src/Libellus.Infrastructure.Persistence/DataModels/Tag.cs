#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Tag : BaseStampedModel<TagId, GroupId>, ISearchable
{
    public UserId? CreatorId { get; set; }

    public string Name { get; set; }
    public string NameNormalized { get; set; }

    public List<BookTagConnector> BookTagConnectors { get; set; } = new List<BookTagConnector>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Tag()
    {
    }

    public Tag(TagId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId? creatorId,
        string name, string nameNormalized) : base(id, groupId, createdOnUtc, modifiedOnUtc)
    {
        CreatorId = creatorId;
        Name = name;
        NameNormalized = nameNormalized;
    }
}