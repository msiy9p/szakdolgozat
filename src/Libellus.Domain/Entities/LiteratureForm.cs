using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class LiteratureForm : BaseShortNamedEntity<LiteratureFormId>, IComparable, IComparable<LiteratureForm>,
    IEquatable<LiteratureForm>
{
    public UserId? CreatorId { get; private set; }

    public ScoreMultiplier ScoreMultiplier { get; private set; }

    internal LiteratureForm(LiteratureFormId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserId? creatorId, ShortName name, ScoreMultiplier scoreMultiplier) : base(id, createdOnUtc, modifiedOnUtc,
        name)
    {
        CreatorId = creatorId;
        ScoreMultiplier = scoreMultiplier;
    }

    public static Result<LiteratureForm> Create(LiteratureFormId id, ZonedDateTime createdOnUtc,
        ZonedDateTime modifiedOnUtc, UserId? creatorId, ShortName name, ScoreMultiplier scoreMultiplier)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<LiteratureForm>.Invalid(result.Errors);
        }

        return Result<LiteratureForm>.Success(new LiteratureForm(id, createdOnUtc, modifiedOnUtc, creatorId, name,
            scoreMultiplier));
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

    public bool ChangeScoreMultiplier(ScoreMultiplier scoreMultiplier, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        ScoreMultiplier = scoreMultiplier;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(LiteratureForm? other)
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

        return obj is LiteratureForm value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is LiteratureForm author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(LiteratureForm? other)
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

    public static bool operator ==(LiteratureForm left, LiteratureForm right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(LiteratureForm left, LiteratureForm right) => !(left == right);

    public static bool operator <(LiteratureForm left, LiteratureForm right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(LiteratureForm left, LiteratureForm right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(LiteratureForm left, LiteratureForm right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(LiteratureForm left, LiteratureForm right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}