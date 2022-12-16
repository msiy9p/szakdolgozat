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

public sealed class Comment : BaseStampedEntity<CommentId>, IComparable, IComparable<Comment>, IEquatable<Comment>
{
    public CommentFriendlyId FriendlyId { get; init; }
    public UserId CreatorId => Creator.UserId;
    public UserPicturedVm Creator { get; private set; }

    public CommentId? RepliedTo { get; private set; }

    public CommentText Text { get; private set; }

    internal Comment(CommentId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        CommentFriendlyId friendlyId, UserPicturedVm creator, CommentText text, CommentId? repliedTo) : base(id,
        createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        Creator = Guard.Against.Null(creator);
        Text = Guard.Against.Null(text);
        RepliedTo = repliedTo;
    }

    public static Result<Comment> Create(CommentId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        CommentFriendlyId friendlyId, UserPicturedVm creator, CommentText text, CommentId? repliedTo = null)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Comment>.Invalid(result.Errors);
        }

        if (creator is null)
        {
            return GeneralErrors.InputIsNull.ToInvalidResult<Comment>();
        }

        if (text is null)
        {
            return GeneralErrors.InputIsNull.ToInvalidResult<Comment>();
        }

        return Result<Comment>.Success(new Comment(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, text,
            repliedTo));
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

    public bool AddRepliedTo(Comment comment, IDateTimeProvider dateTimeProvider)
    {
        if (comment is null || dateTimeProvider is null || RepliedTo.HasValue)
        {
            return false;
        }

        RepliedTo = comment.Id;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveRepliedTo(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!RepliedTo.HasValue)
        {
            return false;
        }

        RepliedTo = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Result<Comment> CreateReply(Comment comment, UserPicturedVm creator, CommentText text,
        IDateTimeProvider dateTimeProvider)
    {
        if (comment is null || creator is null || text is null || dateTimeProvider is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<Comment>();
        }

        return CreateReply(comment.Id, creator, text, dateTimeProvider);
    }

    public Result<Comment> CreateReply(CommentId commentId, UserPicturedVm creator, CommentText text,
        IDateTimeProvider dateTimeProvider)
    {
        if (creator is null || text is null || dateTimeProvider is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<Comment>();
        }

        var date = dateTimeProvider.UtcNow;

        return Create(CommentId.Create(), date, date, CommentFriendlyId.Create(), creator, text, commentId);
    }

    public override string ToString() => Text.ToString();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Comment? other)
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

        return obj is Comment value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Comment author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Comment? other)
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

    public static bool operator ==(Comment left, Comment right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Comment left, Comment right) => !(left == right);

    public static bool operator <(Comment left, Comment right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Comment left, Comment right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Comment left, Comment right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Comment left, Comment right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
}