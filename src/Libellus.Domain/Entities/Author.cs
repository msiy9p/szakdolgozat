using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Author : BaseNamedEntity<AuthorId>, IComparable, IComparable<Author>, IEquatable<Author>
{
    public AuthorFriendlyId FriendlyId { get; init; }
    public UserId? CreatorId => Creator?.UserId;
    public UserVm? Creator { get; private set; }

    public CoverImageMetaDataContainer? AvailableCovers { get; private set; }

    internal Author(AuthorId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, AuthorFriendlyId friendlyId,
        UserVm? creator, Name name, CoverImageMetaDataContainer? availableCovers) : base(id, createdOnUtc,
        modifiedOnUtc, name)
    {
        FriendlyId = friendlyId;
        Creator = creator;
        AvailableCovers = availableCovers;
    }

    public static Result<Author> Create(AuthorId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        AuthorFriendlyId friendlyId, UserVm? creator, Name name, CoverImageMetaDataContainer? availableCovers = null)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<Author>.Invalid(result.Errors);
        }

        return Result<Author>.Success(new Author(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, name,
            availableCovers));
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

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Author? other)
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

        return obj is Author value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Author author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Author? other)
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

    public static bool operator ==(Author left, Author right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Author left, Author right) => !(left == right);

    public static bool operator <(Author left, Author right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Author left, Author right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Author left, Author right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Author left, Author right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}