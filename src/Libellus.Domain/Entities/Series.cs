using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using NodaTime;
using System.Linq.Expressions;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class Series : BaseTitledEntity<SeriesId>, IComparable, IComparable<Series>, IEquatable<Series>
{
    private readonly EntityTrackingContainer<Book, BookId> _booksTracked = new();

    public SeriesFriendlyId FriendlyId { get; init; }
    public UserId? CreatorId => Creator?.UserId;
    public UserVm? Creator { get; private set; }

    private int _bookCountOffset = 0;
    public int BookCount => _booksTracked.Count + _bookCountOffset;
    public IReadOnlyCollection<Book> Books => _booksTracked.GetItems();

    internal Series(SeriesId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, SeriesFriendlyId friendlyId,
        UserVm? creator, Title title) : base(id, createdOnUtc, modifiedOnUtc, title)
    {
        Creator = creator;
        FriendlyId = friendlyId;
    }

    public static Result<Series> Create(SeriesId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        SeriesFriendlyId friendlyId, UserVm? creator, Title title)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, title);
        if (result.IsError)
        {
            return Result<Series>.Invalid(result.Errors);
        }

        return Result<Series>.Success(new Series(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, title));
    }

    public static Result<Series> Create(SeriesId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        SeriesFriendlyId friendlyId, UserVm? creator, Title title, IEnumerable<Book> books)
    {
        if (books is null)
        {
            return Result<Series>.Invalid(SeriesErrors.ProvidedBooksNull);
        }

        var seriesResult = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, title);

        if (seriesResult.IsError)
        {
            return seriesResult;
        }

        var series = seriesResult.Value!;
        foreach (var book in books)
        {
            series._booksTracked.Add(book);
        }

        series._booksTracked.ClearChanges();

        return Result<Series>.Success(series);
    }

    public IReadOnlyEntityTrackingContainer<Book, BookId> GetBookTracker() => _booksTracked;

    public void ResetBookCount()
    {
        _bookCountOffset = 0;
    }

    public void SetBookCountOffset(int offset)
    {
        if (offset < 0)
        {
            return;
        }

        _bookCountOffset = offset;
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

    public bool AddBook(Book book, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (book.Series is null || book.NumberInSeries is null)
        {
            return false;
        }

        if (book.Series.Id != Id)
        {
            return false;
        }

        if (!_booksTracked.Add(book))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public void AddBookRange(IEnumerable<Book> books, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return;
        }

        if (books is null)
        {
            return;
        }

        foreach (var item in books)
        {
            if (item.Series is null || item.NumberInSeries is null)
            {
                continue;
            }

            if (item.Series.Id != Id)
            {
                continue;
            }

            _booksTracked.Add(item);
        }

        UpdateModifiedOnUtc(dateTimeProvider);
    }

    public bool RemoveBook(Book book, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (book is null)
        {
            return false;
        }

        return RemoveBookById(book.Id, dateTimeProvider);
    }

    public bool RemoveBookById(BookId bookId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_booksTracked.Remove(bookId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Book? FindBookById(BookId bookId)
    {
        if (!_booksTracked.Contains(bookId))
        {
            return null;
        }

        return _booksTracked.Find(x => x.Id == bookId);
    }

    public Book? Find(Expression<Func<Book, bool>> predicate)
    {
        if (predicate is null)
        {
            return null;
        }

        return _booksTracked.Find(predicate);
    }

    public ICollection<Book> FindAll(Expression<Func<Book, bool>> predicate)
    {
        if (predicate is null)
        {
            return Array.Empty<Book>();
        }

        return _booksTracked.FindAll(predicate);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Series? other)
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

        return obj is Series value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Series author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Series? other)
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

    public static bool operator ==(Series left, Series right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Series left, Series right) => !(left == right);

    public static bool operator <(Series left, Series right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Series left, Series right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Series left, Series right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Series left, Series right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}