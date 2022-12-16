#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Author : BaseStampedModel<AuthorId, GroupId>, ISearchable
{
    public string FriendlyId { get; set; }
    public UserId? CreatorId { get; set; }
    public CoverImageId? CoverImageId { get; set; }

    public string Name { get; set; }
    public string NameNormalized { get; set; }

    public List<BookAuthorConnector> BookAuthorConnectors { get; set; } = new List<BookAuthorConnector>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Author()
    {
    }

    public Author(AuthorId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        string friendlyId, UserId? creatorId, CoverImageId? coverImageId, string name, string nameNormalized) : base(id,
        groupId, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        CoverImageId = coverImageId;
        Name = name;
        NameNormalized = nameNormalized;
    }
}