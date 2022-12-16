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

public sealed class Shelf : BaseNamedEntity<ShelfId>, IComparable, IComparable<Shelf>, IEquatable<Shelf>
{
    private readonly EntityTrackingContainer<Book, BookId> _booksTracked = new();

    public ShelfFriendlyId FriendlyId { get; init; }
    public UserId? CreatorId => Creator?.UserId;
    public UserVm? Creator { get; private set; }

    public DescriptionText? Description { get; private set; }

    public bool IsLocked { get; set; }

    private int _bookCountOffset = 0;
    public int BookCount => _booksTracked.Count + _bookCountOffset;
    public IReadOnlyCollection<Book> Books => _booksTracked.GetItems();

    internal Shelf(ShelfId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, ShelfFriendlyId friendlyId,
        UserVm? creator, Name name, DescriptionText? description, bool isLocked) : base(id, createdOnUtc,
        modifiedOnUtc, name)
    {
        FriendlyId = friendlyId;
        Creator = creator;
        Description = description;
        IsLocked = isLocked;
    }

    public static Result<Shelf> Create(ShelfId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ShelfFriendlyId friendlyId, UserVm? creator, Name name, DescriptionText? description, bool isLocked)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<Shelf>.Invalid(result.Errors);
        }

        return Result<Shelf>.Success(new Shelf(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, name,
            description, isLocked));
    }

    public static Result<Shelf> Create(ShelfId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ShelfFriendlyId friendlyId, UserVm? creator, Name name, DescriptionText? description, bool isLocked,
        IEnumerable<Book> books)
    {
        if (books is null)
        {
            return Result<Shelf>.Invalid(ShelfErrors.ProvidedBooksNull);
        }

        var shelfResult = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, name, description, isLocked);

        if (shelfResult.IsError)
        {
            return shelfResult;
        }

        var shelf = shelfResult.Value!;
        foreach (var book in books)
        {
            shelf._booksTracked.Add(book);
        }

        shelf._booksTracked.ClearChanges();

        return Result<Shelf>.Success(shelf);
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

    public bool MarkAsLocked(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsLocked)
        {
            return false;
        }

        IsLocked = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsOpen(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsLocked)
        {
            return false;
        }

        IsLocked = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeLockStatus(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsLocked(dateTimeProvider) : MarkAsOpen(dateTimeProvider);

    public bool AddBook(Book book, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
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

    public bool Equals(Shelf? other)
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

        return obj is Shelf value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Shelf author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Shelf? other)
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

    public static bool operator ==(Shelf left, Shelf right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Shelf left, Shelf right) => !(left == right);

    public static bool operator <(Shelf left, Shelf right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Shelf left, Shelf right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Shelf left, Shelf right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Shelf left, Shelf right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}