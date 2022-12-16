using Libellus.Domain.Common.Interfaces.Events;
using NodaTime;
using System.Text.Json.Serialization;

namespace Libellus.Domain.Common.Events;

public class BaseDomainEvent : IDomainEvent
{
    [JsonConstructor]
    public BaseDomainEvent(ZonedDateTime dateOccurredOnUtc)
    {
        DateOccurredOnUtc = dateOccurredOnUtc;
    }

    [JsonInclude] public ZonedDateTime DateOccurredOnUtc { get; private init; }
}