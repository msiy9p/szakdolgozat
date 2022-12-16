using MediatR;
using NodaTime;

namespace Libellus.Domain.Common.Interfaces.Events;

public interface IDomainEvent : INotification
{
    ZonedDateTime DateOccurredOnUtc { get; }
}