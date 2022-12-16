using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Events;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class Post : BaseStampedEntity<PostId>, IComparable, IComparable<Post>, IEquatable<Post>
{
    public static readonly Duration LockAfterDays = Duration.FromDays(31 * 3);

    public const bool IsMemberOnlyDefault = false;
    public const bool IsSpoilerDefault = false;

    private readonly EntityTrackingContainer<Comment, CommentId> _commentsTracked = new();

    public PostFriendlyId FriendlyId { get; init; }
    public UserId? CreatorId => Creator?.UserId;
    public UserPicturedVm? Creator { get; private set; }

    public Title Title { get; init; }
    public CommentText Text { get; private set; }
    public bool IsMemberOnly { get; private set; }
    public bool IsSpoiler { get; private set; }
    public bool IsLocked { get; private set; }

    public Label? Label { get; private set; }

    private int _commentCountOffset = 0;
    public int CommentCount => _commentsTracked.Count + _commentCountOffset;
    public IReadOnlyCollection<Comment> Comments => _commentsTracked.GetItems();

    internal Post(PostId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, PostFriendlyId friendlyId,
        UserPicturedVm? creator, Label? label, Title title, CommentText text, bool isMemberOnly = IsMemberOnlyDefault,
        bool isSpoiler = IsSpoilerDefault, bool isLocked = false) : base(id, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        Creator = creator;
        Title = Guard.Against.Null(title);
        Label = label;
        Text = Guard.Against.Null(text);
        IsMemberOnly = isMemberOnly;
        IsSpoiler = isSpoiler;
        IsLocked = isLocked;
    }

    public static Result<Post> Create(PostId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        PostFriendlyId friendlyId, UserPicturedVm? creator, Label? label, Title title, CommentText text,
        bool isMemberOnly = IsMemberOnlyDefault, bool isSpoiler = IsSpoilerDefault, bool isLocked = false)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Post>.Invalid(result.Errors);
        }

        if (title is null)
        {
            return Result<Post>.Invalid(PostErrors.ProvidedTitleNull);
        }

        if (text is null)
        {
            return Result<Post>.Invalid(GeneralErrors.InputIsNull);
        }

        return Result<Post>.Success(new Post(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, label, title, text,
            isMemberOnly, isSpoiler, isLocked));
    }


    public static Result<Post> Create(PostId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        PostFriendlyId friendlyId, UserPicturedVm? creator, Label? label, Title title, CommentText text,
        IEnumerable<Comment> comments, bool isMemberOnly = IsMemberOnlyDefault, bool isSpoiler = IsSpoilerDefault,
        bool isLocked = false)
    {
        if (comments is null)
        {
            return Result<Post>.Invalid(PostErrors.ProvidedCommentsNull);
        }

        var postResult = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, label, title, text,
            isMemberOnly, isSpoiler, isLocked);

        if (postResult.IsError)
        {
            return postResult;
        }

        var post = postResult.Value!;
        foreach (var comment in comments)
        {
            post._commentsTracked.Add(comment);
        }

        post._commentsTracked.ClearChanges();

        return Result<Post>.Success(post);
    }

    public IReadOnlyEntityTrackingContainer<Comment, CommentId> GetCommentTracker() => _commentsTracked;

    public void ResetCommentCount()
    {
        _commentCountOffset = 0;
    }

    public void SetCommentCountOffset(int offset)
    {
        if (offset < 0)
        {
            return;
        }

        _commentCountOffset = offset;
    }

    public bool Unlock(UserId userId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (AddDomainEvent(new PostUnlockedEvent(dateTimeProvider.UtcNow, this.Id, userId)))
        {
            IsLocked = false;
            UpdateModifiedOnUtc(dateTimeProvider);
            return true;
        }

        return false;
    }

    public bool Lock(UserId userId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (AddDomainEvent(new PostLockedEvent(dateTimeProvider.UtcNow, this.Id, userId)))
        {
            IsLocked = true;
            UpdateModifiedOnUtc(dateTimeProvider);
            return true;
        }

        return false;
    }

    public bool Lock(UserId userId, CommentText lockReason, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (lockReason is null)
        {
            return false;
        }

        if (AddDomainEvent(new PostLockedEvent(dateTimeProvider.UtcNow, this.Id, userId, lockReason)))
        {
            IsLocked = true;
            UpdateModifiedOnUtc(dateTimeProvider);
            return true;
        }

        return false;
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

    public bool MarkAsSpoiler(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsSpoiler)
        {
            return false;
        }

        IsSpoiler = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsNonSpoiler(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsSpoiler)
        {
            return false;
        }

        IsSpoiler = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeSpoiler(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsSpoiler(dateTimeProvider) : MarkAsNonSpoiler(dateTimeProvider);

    public bool MarkAsMemberOnly(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsMemberOnly)
        {
            return false;
        }

        IsMemberOnly = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsPubliclyReadable(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsMemberOnly)
        {
            return false;
        }

        IsMemberOnly = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeMemberOnly(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsMemberOnly(dateTimeProvider) : MarkAsPubliclyReadable(dateTimeProvider);

    public bool RemoveLabel(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Label = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeLabel(Label label, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (label is null)
        {
            return false;
        }

        Label = label;

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

    public bool AddComment(Comment comment)
    {
        if (comment is null)
        {
            return false;
        }

        return _commentsTracked.Add(comment);
    }

    public bool RemoveComment(Comment comment)
    {
        if (comment is null)
        {
            return false;
        }

        return RemoveCommentById(comment.Id);
    }

    public bool RemoveCommentById(CommentId commentId)
    {
        return _commentsTracked.Remove(commentId);
    }

    public override string ToString() => Title.ToString();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Post? other)
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

        return obj is Post value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Post author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Post? other)
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

    public static bool operator ==(Post left, Post right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Post left, Post right) => !(left == right);

    public static bool operator <(Post left, Post right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Post left, Post right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Post left, Post right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Post left, Post right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;

    public class CompareByTitle : Comparer<Post>
    {
        public override int Compare(Post? x, Post? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            var result = new Title.CompareByTitle().Compare(x!.Title, y!.Title);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtc().Compare(x, y);
        }
    }

    public class CompareByTitleDesc : Comparer<Post>
    {
        public override int Compare(Post? x, Post? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            var result = new Title.CompareByTitleDesc().Compare(x!.Title, y!.Title);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtcDesc().Compare(x, y);
        }
    }
}