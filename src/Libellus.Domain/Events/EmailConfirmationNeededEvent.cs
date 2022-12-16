using Ardalis.GuardClauses;
using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Domain.Events;

public sealed class EmailConfirmationNeededEvent : BaseDomainEvent, IEquatable<EmailConfirmationNeededEvent>
{
    public UserId UserId { get; init; }
    public string EmailConfirmationUrl { get; init; }

    public EmailConfirmationNeededEvent(ZonedDateTime dateOccurredOnUtc, UserId userId, string emailConfirmationUrl) : base(
        dateOccurredOnUtc)
    {
        UserId = userId;
        EmailConfirmationUrl = Guard.Against.NullOrWhiteSpace(emailConfirmationUrl);
    }

    public bool Equals(EmailConfirmationNeededEvent? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return UserId.Equals(other.UserId);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is EmailConfirmationNeededEvent other && Equals(other);

    public override int GetHashCode() => UserId.GetHashCode();

    public static bool operator ==(EmailConfirmationNeededEvent? left, EmailConfirmationNeededEvent? right) =>
        Equals(left, right);

    public static bool operator !=(EmailConfirmationNeededEvent? left, EmailConfirmationNeededEvent? right) =>
        !Equals(left, right);
}