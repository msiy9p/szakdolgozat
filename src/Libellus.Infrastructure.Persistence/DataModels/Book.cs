#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Book : BaseStampedModel<BookId, GroupId>, ISearchable
{
    public string FriendlyId { get; set; }
    public UserId? CreatorId { get; set; }
    public CoverImageId? CoverImageId { get; set; }

    public string Title { get; set; }
    public string TitleNormalized { get; set; }
    public string? Description { get; set; }
    public LiteratureFormId? LiteratureFormId { get; set; }

    public LiteratureForm? LiteratureForm { get; set; }
    public BookSeriesConnector? BookSeriesConnector { get; set; }

    public List<BookEdition> BookEditions { get; set; } = new List<BookEdition>();

    public List<BookAuthorConnector> BookAuthorConnectors { get; set; } = new List<BookAuthorConnector>();
    public List<BookGenreConnector> BookGenreConnectors { get; set; } = new List<BookGenreConnector>();
    public List<BookTagConnector> BookTagConnectors { get; set; } = new List<BookTagConnector>();
    public List<BookWarningTagConnector> BookWarningTagConnectors { get; set; } = new List<BookWarningTagConnector>();
    public List<ShelfBookConnector> ShelfBookConnectors { get; set; } = new List<ShelfBookConnector>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Book()
    {
    }

    public Book(BookId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, string friendlyId,
        UserId? creatorId, CoverImageId? coverImageId, string title, string titleNormalized, string? description,
        LiteratureFormId? literatureFormId) : base(id, groupId, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        CoverImageId = coverImageId;
        Title = title;
        TitleNormalized = titleNormalized;
        Description = description;
        LiteratureFormId = literatureFormId;
    }
}