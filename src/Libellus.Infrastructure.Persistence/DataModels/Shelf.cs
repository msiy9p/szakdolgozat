#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Shelf : BaseStampedModel<ShelfId, GroupId>, ISearchable
{
    public string FriendlyId { get; set; }
    public UserId? CreatorId { get; set; }
    public string Name { get; set; }
    public string NameNormalized { get; set; }
    public string? Description { get; set; }
    public bool IsLocked { get; set; }

    public List<ShelfBookConnector> ShelfBookConnectors { get; set; } = new List<ShelfBookConnector>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Shelf()
    {
    }

    public Shelf(ShelfId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        string friendlyId, UserId? creatorId, string name, string nameNormalized, string? description,
        bool isLocked) : base(id, groupId, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        Name = name;
        NameNormalized = nameNormalized;
        Description = description;
        IsLocked = isLocked;
    }
}