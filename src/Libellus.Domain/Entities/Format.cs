using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Format : BaseShortNamedEntity<FormatId>, IComparable, IComparable<Format>, IEquatable<Format>
{
    public const bool IsDigitalDefault = false;

    public UserId? CreatorId { get; private set; }

    public bool IsDigital { get; private set; }

    internal Format(FormatId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId? creatorId,
        ShortName name, bool isDigital = IsDigitalDefault) : base(id, createdOnUtc, modifiedOnUtc, name)
    {
        CreatorId = creatorId;
        IsDigital = isDigital;
    }

    public static Result<Format> Create(FormatId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId? creatorId, ShortName name, bool isDigital = IsDigitalDefault)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<Format>.Invalid(result.Errors);
        }

        return Result<Format>.Success(new Format(id, createdOnUtc, modifiedOnUtc, creatorId, name, isDigital));
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

    public bool MarkAsDigital(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        IsDigital = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsNotDigital(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        IsDigital = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Format? other)
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

        return obj is Format value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Format author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Format? other)
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

    public static bool operator ==(Format left, Format right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Format left, Format right) => !(left == right);

    public static bool operator <(Format left, Format right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Format left, Format right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Format left, Format right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Format left, Format right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}