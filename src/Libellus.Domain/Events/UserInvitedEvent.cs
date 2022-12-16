using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Domain.Events;

public sealed class UserInvitedEvent : BaseDomainEvent, IEquatable<UserInvitedEvent>
{
    public InvitationId InvitationId { get; init; }

    public UserInvitedEvent(ZonedDateTime dateOccurredOnUtc, InvitationId invitationId) : base(dateOccurredOnUtc)
    {
        InvitationId = invitationId;
    }

    public bool Equals(UserInvitedEvent? other)
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
        ReferenceEquals(this, obj) || obj is UserInvitedEvent other && Equals(other);

    public override int GetHashCode() => InvitationId.GetHashCode();

    public static bool operator ==(UserInvitedEvent? left, UserInvitedEvent? right) => Equals(left, right);

    public static bool operator !=(UserInvitedEvent? left, UserInvitedEvent? right) => !Equals(left, right);
}