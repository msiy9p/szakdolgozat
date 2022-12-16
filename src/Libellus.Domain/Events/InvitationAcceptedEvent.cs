using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Domain.Events;

public class InvitationAcceptedEvent : BaseDomainEvent, IEquatable<InvitationAcceptedEvent>
{
    public InvitationId InvitationId { get; init; }
    public UserId UserId { get; init; }
    public GroupId GroupId { get; init; }

    public InvitationAcceptedEvent(ZonedDateTime dateOccurredOnUtc, InvitationId invitationId, UserId userId,
        GroupId groupId) : base(dateOccurredOnUtc)
    {
        InvitationId = invitationId;
        UserId = userId;
        GroupId = groupId;
    }

    public bool Equals(InvitationAcceptedEvent? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return InvitationId.Equals(other.InvitationId);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is InvitationAcceptedEvent other && Equals(other);

    public override int GetHashCode() => InvitationId.GetHashCode();

    public static bool operator ==(InvitationAcceptedEvent? left, InvitationAcceptedEvent? right) =>
        Equals(left, right);

    public static bool operator !=(InvitationAcceptedEvent? left, InvitationAcceptedEvent? right) =>
        !Equals(left, right);
}