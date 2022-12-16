using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class BookEdition : BaseTitledEntity<BookEditionId>, IComparable, IComparable<BookEdition>,
    IEquatable<BookEdition>
{
    private readonly EntityTrackingContainer<Reading, ReadingId> _readingsTracked = new();

    public BookEditionFriendlyId FriendlyId { get; init; }
    public UserId? CreatorId => Creator?.UserId;
    public UserVm? Creator { get; private set; }

    public BookId BookId => Book.BookId;
    public BookCompactVm Book { get; init; }

    public DescriptionText? Description { get; private set; }

    public PartialDate? PublishedOn { get; private set; }
    public bool IsTranslation { get; private set; }
    public PageCount? PageCount { get; private set; }
    public WordCount? WordCount { get; private set; }
    public Isbn? Isbn { get; private set; }

    public Format? Format { get; private set; }
    public Language? Language { get; private set; }
    public Publisher? Publisher { get; private set; }
    public CoverImageMetaDataContainer? AvailableCovers { get; private set; }

    private int _readingCountOffset = 0;
    public int ReadingCount => _readingsTracked.Count + _readingCountOffset;
    public IReadOnlyCollection<Reading> Readings => _readingsTracked.GetItems();

    internal BookEdition(BookEditionId id,
        ZonedDateTime createdOnUtc,
        ZonedDateTime modifiedOnUtc,
        BookEditionFriendlyId friendlyId,
        UserVm? creator,
        BookCompactVm book,
        Title title,
        DescriptionText? description,
        Format? format,
        Language? language,
        Publisher? publisher,
        PartialDate? publishedOn,
        bool isTranslation,
        PageCount? pageCount,
        WordCount? wordCount,
        Isbn? isbn,
        CoverImageMetaDataContainer? availableCovers) :
        base(id, createdOnUtc, modifiedOnUtc, title)
    {
        FriendlyId = friendlyId;
        Creator = creator;
        Book = book;
        Description = description;
        Format = format;
        Language = language;
        Publisher = publisher;
        PageCount = pageCount;
        WordCount = wordCount;
        Isbn = isbn;

        PublishedOn = publishedOn;
        IsTranslation = isTranslation;
        AvailableCovers = availableCovers;
    }

    public static Result<BookEdition> Create(BookEditionId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        BookEditionFriendlyId friendlyId, UserVm? creator, BookCompactVm book, Title title,
        DescriptionText? description,
        bool isTranslation, CoverImageMetaDataContainer? availableCovers = null)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, title);
        if (result.IsError)
        {
            return Result<BookEdition>.Invalid(result.Errors);
        }

        if (book is null)
        {
            return GeneralErrors.InputIsNull.ToInvalidResult<BookEdition>();
        }

        return Result<BookEdition>.Success(new BookEdition(id, createdOnUtc, modifiedOnUtc, friendlyId, creator,
            book, title, description, null, null, null, null, isTranslation,
            null, null, null, availableCovers));
    }

    public static Result<BookEdition> Create(BookEditionId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        BookEditionFriendlyId friendlyId, UserVm? creator, BookCompactVm book, Title title,
        DescriptionText? description,
        Format? format, Language? language, Publisher? publisher, PartialDate? publishedOn, bool isTranslation,
        PageCount? pageCount, WordCount? wordCount, Isbn? isbn, CoverImageMetaDataContainer? availableCovers)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, title);
        if (result.IsError)
        {
            return Result<BookEdition>.Invalid(result.Errors);
        }

        return Result<BookEdition>.Success(new BookEdition(id, createdOnUtc, modifiedOnUtc, friendlyId, creator,
            book, title, description, format, language, publisher, publishedOn, isTranslation,
            pageCount, wordCount, isbn, availableCovers));
    }

    public static Result<BookEdition> Create(BookEditionId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        BookEditionFriendlyId friendlyId, UserVm? creator, BookCompactVm book, Title title,
        DescriptionText? description,
        Format? format, Language? language, Publisher? publisher, PartialDate? publishedOn, bool isTranslation,
        PageCount? pageCount, WordCount? wordCount, Isbn? isbn, CoverImageMetaDataContainer? availableCovers,
        IEnumerable<Reading> readings)
    {
        if (readings is null)
        {
            return Result<BookEdition>.Invalid(BookEditionErrors.ProvidedReadingsNull);
        }

        var bookEditionResult = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, book, title,
            description, format, language, publisher, publishedOn, isTranslation, pageCount, wordCount,
            isbn, availableCovers);
        if (bookEditionResult.IsError)
        {
            return bookEditionResult;
        }

        var bookEdition = bookEditionResult.Value!;
        foreach (var reading in readings)
        {
            bookEdition._readingsTracked.Add(reading);
        }

        bookEdition._readingsTracked.ClearChanges();

        return Result<BookEdition>.Success(bookEdition);
    }

    public IReadOnlyEntityTrackingContainer<Reading, ReadingId> GetReadingTracker() => _readingsTracked;

    public void ResetReadingCount()
    {
        _readingCountOffset = 0;
    }

    public void SetReadingCountOffset(int offset)
    {
        if (offset < 0)
        {
            return;
        }

        _readingCountOffset = offset;
    }

    public bool RemoveCreator(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Creator = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveDescription(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Description = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeDescription(string description, IDateTimeProvider dateTimeProvider)
    {
        if (!DescriptionText.IsValidDescriptionText(description))
        {
            return false;
        }

        return ChangeDescription(new DescriptionText(description), dateTimeProvider);
    }

    public bool ChangeDescription(DescriptionText description, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (description is null)
        {
            return false;
        }

        Description = description;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveCovers(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        AvailableCovers = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeCovers(CoverImageMetaDataContainer availableCovers, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (availableCovers is null || availableCovers.Count < 1)
        {
            return false;
        }

        AvailableCovers = availableCovers;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveFormat(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Format = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeFormat(Format format, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (format is null)
        {
            return false;
        }

        Format = format;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveLanguage(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Language = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeLanguage(Language language, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (language is null)
        {
            return false;
        }

        Language = language;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsTranslation(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsTranslation)
        {
            return false;
        }

        IsTranslation = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsNotTranslation(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsTranslation)
        {
            return false;
        }

        IsTranslation = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeTranslation(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsTranslation(dateTimeProvider) : MarkAsNotTranslation(dateTimeProvider);

    public bool RemovePublisher(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Publisher = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangePublisher(Publisher publisher, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (publisher is null)
        {
            return false;
        }

        Publisher = publisher;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemovePublishedOn(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        PublishedOn = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangePublishedOn(PartialDate partialDate, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        PublishedOn = partialDate;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemovePageCount(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        PageCount = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangePageCount(PageCount pageCount, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        PageCount = pageCount;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveWordCount(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        WordCount = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeWordCount(WordCount wordCount, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        WordCount = wordCount;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveIsbn(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Isbn = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeIsbn(Isbn isbn, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Isbn = isbn;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool AddReading(Reading reading)
    {
        if (reading is null)
        {
            return false;
        }

        return _readingsTracked.Add(reading);
    }

    public bool RemoveReading(Reading reading)
    {
        if (reading is null)
        {
            return false;
        }

        return RemoveReadingById(reading.Id);
    }

    public bool RemoveReadingById(ReadingId readingId)
    {
        return _readingsTracked.Remove(readingId);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(BookEdition? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        return obj is BookEdition value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is BookEdition bookEdition)
        {
            return CompareTo(bookEdition);
        }

        return 1;
    }

    public int CompareTo(BookEdition? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        return Id.CompareTo(other.Id);
    }

    public static bool operator ==(BookEdition left, BookEdition right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(BookEdition left, BookEdition right) => !(left == right);

    public static bool operator <(BookEdition left, BookEdition right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(BookEdition left, BookEdition right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(BookEdition left, BookEdition right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(BookEdition left, BookEdition right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;

    public class CompareByPageCount : Comparer<BookEdition>
    {
        public override int Compare(BookEdition? x, BookEdition? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.PageCount.HasValue && !y!.PageCount.HasValue)
            {
                result = 0;
            }
            else if (x!.PageCount.HasValue && !y!.PageCount.HasValue)
            {
                result = 1;
            }
            else if (!x!.PageCount.HasValue && y!.PageCount.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.PageCount!.Value.CompareTo(y!.PageCount!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByTitle().Compare(x, y);
        }
    }

    public class CompareByPageCountDesc : Comparer<BookEdition>
    {
        public override int Compare(BookEdition? x, BookEdition? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.PageCount.HasValue && !y!.PageCount.HasValue)
            {
                result = 0;
            }
            else if (x!.PageCount.HasValue && !y!.PageCount.HasValue)
            {
                result = 1;
            }
            else if (!x!.PageCount.HasValue && y!.PageCount.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.PageCount!.Value.CompareTo(y!.PageCount!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByTitleDesc().Compare(x, y);
        }
    }

    public class CompareByWordCount : Comparer<BookEdition>
    {
        public override int Compare(BookEdition? x, BookEdition? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.WordCount.HasValue && !y!.WordCount.HasValue)
            {
                result = 0;
            }
            else if (x!.WordCount.HasValue && !y!.WordCount.HasValue)
            {
                result = 1;
            }
            else if (!x!.WordCount.HasValue && y!.WordCount.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.WordCount!.Value.CompareTo(y!.WordCount!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByTitle().Compare(x, y);
        }
    }

    public class CompareByWordCountDesc : Comparer<BookEdition>
    {
        public override int Compare(BookEdition? x, BookEdition? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.WordCount.HasValue && !y!.WordCount.HasValue)
            {
                result = 0;
            }
            else if (x!.WordCount.HasValue && !y!.WordCount.HasValue)
            {
                result = 1;
            }
            else if (!x!.WordCount.HasValue && y!.WordCount.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.WordCount!.Value.CompareTo(y!.WordCount!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByTitleDesc().Compare(x, y);
        }
    }

    public class CompareByPublishedOn : Comparer<BookEdition>
    {
        public override int Compare(BookEdition? x, BookEdition? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.PublishedOn.HasValue && !y!.PublishedOn.HasValue)
            {
                result = 0;
            }
            else if (x!.PublishedOn.HasValue && !y!.PublishedOn.HasValue)
            {
                result = 1;
            }
            else if (!x!.PublishedOn.HasValue && y!.PublishedOn.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.PublishedOn!.Value.CompareTo(y!.PublishedOn!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByTitle().Compare(x, y);
        }
    }

    public class CompareByPublishedOnDesc : Comparer<BookEdition>
    {
        public override int Compare(BookEdition? x, BookEdition? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.PublishedOn.HasValue && !y!.PublishedOn.HasValue)
            {
                result = 0;
            }
            else if (x!.PublishedOn.HasValue && !y!.PublishedOn.HasValue)
            {
                result = 1;
            }
            else if (!x!.PublishedOn.HasValue && y!.PublishedOn.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.PublishedOn!.Value.CompareTo(y!.PublishedOn!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByTitleDesc().Compare(x, y);
        }
    }
}