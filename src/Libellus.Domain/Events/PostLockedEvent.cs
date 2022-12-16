using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using NodaTime;

namespace Libellus.Domain.Events;

public sealed class PostLockedEvent : BaseDomainEvent, IEquatable<PostLockedEvent>
{
    public PostId PostId { get; init; }
    public UserId? UserId { get; init; }
    public CommentText? LockReason { get; init; }

    public PostLockedEvent(ZonedDateTime dateOccurredOnUtc, PostId postId, UserId? userId) :
        base(dateOccurredOnUtc)
    {
        PostId = postId;
        UserId = userId;
        LockReason = null;
    }

    public PostLockedEvent(ZonedDateTime dateOccurredOnUtc, PostId postId, UserId? userId, CommentText lockReason) :
        base(dateOccurredOnUtc)
    {
        PostId = postId;
        UserId = userId;
        LockReason = lockReason;
    }

    public bool Equals(PostLockedEvent? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return PostId.Equals(other.PostId);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is PostLockedEvent other && Equals(other);

    public override int GetHashCode() => PostId.GetHashCode();

    public static bool operator ==(PostLockedEvent? left, PostLockedEvent? right) =>
        Equals(left, right);

    public static bool operator !=(PostLockedEvent? left, PostLockedEvent? right) =>
        !Equals(left, right);
}