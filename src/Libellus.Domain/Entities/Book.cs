using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class Book : BaseTitledEntity<BookId>, IComparable, IComparable<Book>, IEquatable<Book>
{
    public const decimal NumberInSeriesMin = 0;

    private readonly EntityTrackingContainer<Author, AuthorId> _authorsTracked = new();
    private readonly EntityTrackingContainer<Genre, GenreId> _genresTracked = new();
    private readonly EntityTrackingContainer<Tag, TagId> _tagsTracked = new();
    private readonly EntityTrackingContainer<WarningTag, WarningTagId> _warningTagsTracked = new();
    private readonly EntityTrackingContainer<BookEdition, BookEditionId> _bookEditionsTracked = new();

    public BookFriendlyId FriendlyId { get; init; }
    public UserId? CreatorId => Creator?.UserId;
    public UserVm? Creator { get; private set; }

    public DescriptionText? Description { get; private set; }
    public decimal? NumberInSeries { get; private set; }

    public LiteratureForm? LiteratureForm { get; private set; }
    public Series? Series { get; private set; }
    public CoverImageMetaDataContainer? AvailableCovers { get; private set; }

    private int _bookEditionCountOffset = 0;
    public int BookEditionCount => _bookEditionsTracked.Count + _bookEditionCountOffset;

    public IReadOnlyCollection<Author> Authors => _authorsTracked.GetItems();
    public IReadOnlyCollection<Genre> Genres => _genresTracked.GetItems();
    public IReadOnlyCollection<Tag> Tags => _tagsTracked.GetItems();
    public IReadOnlyCollection<WarningTag> WarningTags => _warningTagsTracked.GetItems();

    public IReadOnlyCollection<BookEdition> BookEditions => _bookEditionsTracked.GetItems();

    internal Book(BookId id,
        ZonedDateTime createdOnUtc,
        ZonedDateTime modifiedOnUtc,
        BookFriendlyId friendlyId,
        UserVm? creator,
        LiteratureForm? literatureForm,
        Series? series,
        Title title,
        DescriptionText? description,
        decimal? numberInSeries,
        CoverImageMetaDataContainer? availableCovers) :
        base(id, createdOnUtc, modifiedOnUtc, title)
    {
        FriendlyId = friendlyId;
        Creator = creator;
        LiteratureForm = literatureForm;
        Series = series;
        Description = description;
        if (!IsValidNumberInSeries(numberInSeries))
        {
            throw new NumberInSeriesInvalidException("Number in series is invalid.", nameof(numberInSeries));
        }

        NumberInSeries = numberInSeries;
        AvailableCovers = availableCovers;
    }

    public static Result<Book> Create(BookId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        BookFriendlyId friendlyId, UserVm? creator, Title title, DescriptionText? description,
        CoverImageMetaDataContainer? availableCovers = null)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, title);
        if (result.IsError)
        {
            return Result<Book>.Invalid(result.Errors);
        }


        return Result<Book>.Success(new Book(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, null,
            null, title, description, null, availableCovers));
    }

    public static Result<Book> Create(BookId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        BookFriendlyId friendlyId, UserVm? creator, LiteratureForm? literatureForm, Series? series, Title title,
        DescriptionText? description, decimal? numberInSeries, CoverImageMetaDataContainer? availableCovers)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, title);
        if (result.IsError)
        {
            return Result<Book>.Invalid(result.Errors);
        }

        if (!IsValidNumberInSeries(numberInSeries))
        {
            return Result<Book>.Invalid(BookErrors.InvalidNumberInSeries);
        }

        return Result<Book>.Success(new Book(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, literatureForm,
            series, title, description, numberInSeries, availableCovers));
    }

    public static Result<Book> Create(BookId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        BookFriendlyId friendlyId, UserVm? creator, LiteratureForm? literatureForm, Series? series, Title title,
        DescriptionText? description, decimal? numberInSeries, CoverImageMetaDataContainer? availableCovers,
        IEnumerable<Author> authors, IEnumerable<Genre> genres, IEnumerable<Tag> tags,
        IEnumerable<WarningTag> warningTags, IEnumerable<BookEdition> bookEditions)
    {
        if (authors is null)
        {
            return Result<Book>.Invalid(BookErrors.ProvidedAuthorsNull);
        }

        if (genres is null)
        {
            return Result<Book>.Invalid(BookErrors.ProvidedGenresNull);
        }

        if (tags is null)
        {
            return Result<Book>.Invalid(BookErrors.ProvidedTagsNull);
        }

        if (warningTags is null)
        {
            return Result<Book>.Invalid(BookErrors.ProvidedWarningTagsNull);
        }

        if (bookEditions is null)
        {
            return Result<Book>.Invalid(BookErrors.ProvidedBookEditionsNull);
        }

        var bookResult = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, literatureForm, series, title,
            description, numberInSeries, availableCovers);
        if (bookResult.IsError)
        {
            return bookResult;
        }

        var book = bookResult.Value!;

        foreach (var author in authors)
        {
            book._authorsTracked.Add(author);
        }

        book._authorsTracked.ClearChanges();

        foreach (var genre in genres)
        {
            if (genre is not null && book._genresTracked.FirstOrDefault(x => x.Name == genre.Name) is null)
            {
                book._genresTracked.Add(genre);
            }
        }

        book._genresTracked.ClearChanges();

        foreach (var tag in tags)
        {
            if (tag is not null && book._tagsTracked.FirstOrDefault(x => x.Name == tag.Name) is null)
            {
                book._tagsTracked.Add(tag);
            }
        }

        book._tagsTracked.ClearChanges();

        foreach (var warningTag in warningTags)
        {
            if (warningTag is not null &&
                book._warningTagsTracked.FirstOrDefault(x => x.Name == warningTag.Name) is null)
            {
                book._warningTagsTracked.Add(warningTag);
            }
        }

        book._warningTagsTracked.ClearChanges();

        foreach (var bookEdition in bookEditions)
        {
            book._bookEditionsTracked.Add(bookEdition);
        }

        book._bookEditionsTracked.ClearChanges();

        return Result<Book>.Success(book);
    }

    public IReadOnlyEntityTrackingContainer<Author, AuthorId> GetAuthorTracker() => _authorsTracked;

    public IReadOnlyEntityTrackingContainer<Genre, GenreId> GetGenreTracker() => _genresTracked;

    public IReadOnlyEntityTrackingContainer<Tag, TagId> GetTagTracker() => _tagsTracked;

    public IReadOnlyEntityTrackingContainer<WarningTag, WarningTagId> GetWarningTagTracker() => _warningTagsTracked;

    public static bool IsValidNumberInSeries(decimal? numberInSeries) =>
        numberInSeries.HasValue ? numberInSeries > NumberInSeriesMin : true;

    public void ResetBookEditionCount()
    {
        _bookEditionCountOffset = 0;
    }

    public void SetBookEditionCountOffset(int offset)
    {
        if (offset < 0)
        {
            return;
        }

        _bookEditionCountOffset = offset;
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

    public bool RemoveLiteratureForm(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        LiteratureForm = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeLiteratureForm(LiteratureForm literatureForm, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (literatureForm is null)
        {
            return false;
        }

        LiteratureForm = literatureForm;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveSeries(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Series = null;
        NumberInSeries = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeSeries(Series series, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (series is null)
        {
            return false;
        }

        Series = series;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveNumberInSeries(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        NumberInSeries = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeNumberInSeries(decimal numberInSeries, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsValidNumberInSeries(numberInSeries))
        {
            return false;
        }

        NumberInSeries = numberInSeries;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
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

    public bool AddAuthor(Author author, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_authorsTracked.Add(author))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveAuthor(Author author, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (author is null)
        {
            return false;
        }

        return RemoveAuthorById(author.Id, dateTimeProvider);
    }

    public bool RemoveAuthorById(AuthorId authorId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_authorsTracked.Remove(authorId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Author? FindAuthorByName(string name)
    {
        if (!Name.IsValidName(name))
        {
            return null;
        }

        return FindAuthorByName(new Name(name));
    }

    public Author? FindAuthorByName(Name name)
    {
        if (name is null)
        {
            return null;
        }

        return _authorsTracked.FirstOrDefault(x => x.Name == name);
    }

    public bool AddGenre(Genre genre, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (_genresTracked.FirstOrDefault(x => x.Name == genre.Name) is not null)
        {
            return false;
        }

        if (!_genresTracked.Add(genre))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveGenre(Genre genre, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (genre is null)
        {
            return false;
        }

        return RemoveGenreById(genre.Id, dateTimeProvider);
    }

    public bool RemoveGenreById(GenreId genreId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_genresTracked.Remove(genreId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Genre? FindGenreByName(string name)
    {
        if (!ShortName.IsValidShortName(name))
        {
            return null;
        }

        return FindGenreByName(new ShortName(name));
    }

    public Genre? FindGenreByName(ShortName name)
    {
        if (name is null)
        {
            return null;
        }

        return _genresTracked.FirstOrDefault(x => x.Name == name);
    }

    public bool AddTag(Tag tag, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (_tagsTracked.FirstOrDefault(x => x.Name == tag.Name) is not null)
        {
            return false;
        }

        if (!_tagsTracked.Add(tag))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveTag(Tag tag, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (tag is null)
        {
            return false;
        }

        return RemoveTagById(tag.Id, dateTimeProvider);
    }

    public bool RemoveTagById(TagId tagId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_tagsTracked.Remove(tagId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Tag? FindTagByName(string name)
    {
        if (!ShortName.IsValidShortName(name))
        {
            return null;
        }

        return FindTagByName(new ShortName(name));
    }

    public Tag? FindTagByName(ShortName name)
    {
        if (name is null)
        {
            return null;
        }

        return _tagsTracked.FirstOrDefault(x => x.Name == name);
    }

    public bool AddWarningTag(WarningTag warningTag, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (_warningTagsTracked.FirstOrDefault(x => x.Name == warningTag.Name) is not null)
        {
            return false;
        }

        if (!_warningTagsTracked.Add(warningTag))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveWarningTag(WarningTag warningTag, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (warningTag is null)
        {
            return false;
        }

        return RemoveWarningTagById(warningTag.Id, dateTimeProvider);
    }

    public bool RemoveWarningTagById(WarningTagId warningTagId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_warningTagsTracked.Remove(warningTagId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public WarningTag? FindWarningTagByName(string name)
    {
        if (!ShortName.IsValidShortName(name))
        {
            return null;
        }

        return FindWarningTagByName(new ShortName(name));
    }

    public WarningTag? FindWarningTagByName(ShortName name)
    {
        if (name is null)
        {
            return null;
        }

        return _warningTagsTracked.FirstOrDefault(x => x.Name == name);
    }

    public bool AddBookEdition(BookEdition bookEdition, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (bookEdition is null)
        {
            return false;
        }

        if (bookEdition.BookId != Id)
        {
            return false;
        }

        if (!_bookEditionsTracked.Add(bookEdition))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveBookEdition(BookEdition bookEdition, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (bookEdition is null)
        {
            return false;
        }

        return RemoveBookEditionById(bookEdition.Id, dateTimeProvider);
    }

    public bool RemoveBookEditionById(BookEditionId bookEditionId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_bookEditionsTracked.Remove(bookEditionId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public BookEdition? FindBookEditionByTitle(string title)
    {
        if (!Title.IsValidTitle(title))
        {
            return null;
        }

        return FindBookEditionByTitle(new Title(title));
    }

    public BookEdition? FindBookEditionByTitle(Title title)
    {
        if (title is null)
        {
            return null;
        }

        return _bookEditionsTracked.FirstOrDefault(x => x.Title == title);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Book? other)
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

        return obj is Book value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Book author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Book? other)
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

    public static bool operator ==(Book left, Book right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Book left, Book right) => !(left == right);

    public static bool operator <(Book left, Book right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Book left, Book right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Book left, Book right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Book left, Book right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}