using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Genre : BaseShortNamedEntity<GenreId>, IComparable, IComparable<Genre>, IEquatable<Genre>
{
    public const bool IsFictionDefault = true;

    public UserId? CreatorId { get; private set; }

    public bool IsFiction { get; private set; }

    internal Genre(GenreId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId? creatorId,
        ShortName name, bool isFiction = IsFictionDefault) : base(id, createdOnUtc, modifiedOnUtc, name)
    {
        CreatorId = creatorId;
        IsFiction = isFiction;
    }

    public static Result<Genre> Create(GenreId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId? creatorId, ShortName name, bool isFiction = IsFictionDefault)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<Genre>.Invalid(result.Errors);
        }

        return Result<Genre>.Success(new Genre(id, createdOnUtc, modifiedOnUtc, creatorId, name, isFiction));
    }

    public bool RemoveCreator(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        CreatorId = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsFiction(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        IsFiction = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsNonFiction(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        IsFiction = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Genre? other)
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

        return obj is Genre value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Genre author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Genre? other)
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

    public static bool operator ==(Genre left, Genre right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Genre left, Genre right) => !(left == right);

    public static bool operator <(Genre left, Genre right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Genre left, Genre right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Genre left, Genre right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Genre left, Genre right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}