using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class Note : BaseStampedEntity<NoteId>, IComparable, IComparable<Note>, IEquatable<Note>
{
    public UserId? CreatorId => Creator?.UserId;
    public UserPicturedVm? Creator { get; private set; }

    public CommentText Text { get; private set; }

    internal Note(NoteId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserPicturedVm? creator,
        CommentText text) : base(id, createdOnUtc, modifiedOnUtc)
    {
        Creator = creator;
        Text = Guard.Against.Null(text);
    }

    public static Result<Note> Create(NoteId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        UserPicturedVm? creator, CommentText text)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Note>.Invalid(result.Errors);
        }

        if (text is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<Note>();
        }

        return Result<Note>.Success(new Note(id, createdOnUtc, modifiedOnUtc, creator, text));
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

    public bool ChangeText(string text, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!CommentText.IsValidCommentText(text))
        {
            return false;
        }

        return ChangeText(new CommentText(text), dateTimeProvider);
    }

    public bool ChangeText(CommentText text, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (text is null)
        {
            return false;
        }

        Text = text;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override string ToString() => Text.ToString();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Note? other)
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

        return obj is Note value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Note author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Note? other)
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

    public static bool operator ==(Note left, Note right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Note left, Note right) => !(left == right);

    public static bool operator <(Note left, Note right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Note left, Note right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Note left, Note right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Note left, Note right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}