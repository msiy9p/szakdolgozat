#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class BookEdition : BaseStampedModel<BookEditionId, GroupId>, ISearchable
{
    public string FriendlyId { get; set; }
    public BookId BookId { get; set; }
    public UserId? CreatorId { get; set; }
    public CoverImageId? CoverImageId { get; set; }

    public string Title { get; set; }
    public string TitleNormalized { get; set; }
    public string? Description { get; set; }
    public FormatId? FormatId { get; set; }
    public LanguageId? LanguageId { get; set; }
    public PublisherId? PublisherId { get; set; }
    public int? PublishedOnYear { get; set; }
    public int? PublishedOnMonth { get; set; }
    public int? PublishedOnDay { get; set; }
    public bool IsTranslation { get; set; }
    public int? PageCount { get; set; }
    public int? WordCount { get; set; }
    public string? Isbn { get; set; }

    public Book Book { get; set; }
    public Format? Format { get; set; }
    public Language? Language { get; set; }
    public Publisher? Publisher { get; set; }

    public List<Reading> Readings { get; set; } = new List<Reading>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Instant? PublishedOn
    {
        get
        {
            if (PublishedOnYear.HasValue && PublishedOnMonth.HasValue && PublishedOnDay.HasValue)
            {
                return new LocalDate(PublishedOnYear!.Value, PublishedOnMonth!.Value, PublishedOnDay!.Value)
                    .AtStartOfDayInZone(DateTimeZone.Utc)
                    .ToInstant();
            }

            return null;
        }

        private set { }
    }

    public BookEdition()
    {
    }

    public BookEdition(BookEditionId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        string friendlyId, BookId bookId, UserId? creatorId, CoverImageId? coverImageId, string title,
        string titleNormalized, string? description, FormatId? formatId, LanguageId? languageId,
        PublisherId? publisherId, int? publishedOnYear, int? publishedOnMonth, int? publishedOnDay, bool isTranslation,
        int? pageCount, int? wordCount, string? isbn) : base(id, groupId, createdOnUtc,
        modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        BookId = bookId;
        CreatorId = creatorId;
        CoverImageId = coverImageId;
        Title = title;
        TitleNormalized = titleNormalized;
        Description = description;
        FormatId = formatId;
        LanguageId = languageId;
        PublisherId = publisherId;
        PublishedOnYear = publishedOnYear;
        PublishedOnMonth = publishedOnMonth;
        PublishedOnDay = publishedOnDay;
        IsTranslation = isTranslation;
        PageCount = pageCount;
        WordCount = wordCount;
        Isbn = isbn;
    }
}