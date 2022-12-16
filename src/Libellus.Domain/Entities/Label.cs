using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Label : BaseShortNamedEntity<LabelId>, IComparable, IComparable<Label>, IEquatable<Label>
{
    internal Label(LabelId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, ShortName name) : base(id,
        createdOnUtc, modifiedOnUtc, name)
    {
    }

    public new static Result<Label> Create(LabelId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ShortName name)
    {
        var result = BaseShortNamedEntity<LabelId>.Create(id, createdOnUtc, modifiedOnUtc, name);
        if (result.IsError)
        {
            return Result<Label>.Invalid(result.Errors);
        }

        return Result<Label>.Success(new Label(id, createdOnUtc, modifiedOnUtc, name));
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Label? other)
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

        return obj is Label value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Label author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Label? other)
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

    public static bool operator ==(Label left, Label right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Label left, Label right) => !(left == right);

    public static bool operator <(Label left, Label right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Label left, Label right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Label left, Label right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Label left, Label right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}