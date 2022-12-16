using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Language : BaseShortNamedEntity<LanguageId>, IComparable, IComparable<Language>,
    IEquatable<Language>
{
    public UserId? CreatorId { get; private set; }

    internal Language(LanguageId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId? creatorId,
        ShortName name) : base(id, createdOnUtc, modifiedOnUtc, name)
    {
        CreatorId = creatorId;
    }

    public static Result<Language> Create(LanguageId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId? creatorId, ShortName name)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<Language>.Invalid(result.Errors);
        }

        return Result<Language>.Success(new Language(id, createdOnUtc, modifiedOnUtc, creatorId, name));
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

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Language? other)
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

        return obj is Language value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Language author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Language? other)
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

    public static bool operator ==(Language left, Language right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Language left, Language right) => !(left == right);

    public static bool operator <(Language left, Language right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Language left, Language right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Language left, Language right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Language left, Language right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}