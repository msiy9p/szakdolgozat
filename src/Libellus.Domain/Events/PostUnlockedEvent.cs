using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Domain.Events;

public sealed class PostUnlockedEvent : BaseDomainEvent, IEquatable<PostUnlockedEvent>
{
    public PostId PostId { get; init; }
    public UserId UserId { get; init; }

    public PostUnlockedEvent(ZonedDateTime dateOccurredOnUtc, PostId postId, UserId userId) :
        base(dateOccurredOnUtc)
    {
        PostId = postId;
        UserId = userId;
    }

    public bool Equals(PostUnlockedEvent? other)
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
        ReferenceEquals(this, obj) || obj is PostUnlockedEvent other && Equals(other);

    public override int GetHashCode() => PostId.GetHashCode();

    public static bool operator ==(PostUnlockedEvent? left, PostUnlockedEvent? right) =>
        Equals(left, right);

    public static bool operator !=(PostUnlockedEvent? left, PostUnlockedEvent? right) =>
        !Equals(left, right);
}