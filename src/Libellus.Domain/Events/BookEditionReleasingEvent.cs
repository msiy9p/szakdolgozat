using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Domain.Events;

public sealed class BookEditionReleasingEvent : BaseDomainEvent, IEquatable<BookEditionReleasingEvent>
{
    public BookEditionId BookEditionId { get; init; }

    public BookEditionReleasingEvent(ZonedDateTime dateOccurredOnUtc, BookEditionId bookEditionId) : base(
        dateOccurredOnUtc)
    {
        BookEditionId = bookEditionId;
    }

    public bool Equals(BookEditionReleasingEvent? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return BookEditionId.Equals(other.BookEditionId);
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is BookEditionReleasingEvent other && Equals(other);

    public override int GetHashCode() => BookEditionId.GetHashCode();

    public static bool operator ==(BookEditionReleasingEvent? left, BookEditionReleasingEvent? right) =>
        Equals(left, right);

    public static bool operator !=(BookEditionReleasingEvent? left, BookEditionReleasingEvent? right) =>
        !Equals(left, right);
}