using Libellus.Domain.Common.Events;

namespace Libellus.Domain.Common.Interfaces.Models;

public interface IHasDomainEvents
{
    bool HasDomainEvents { get; }
    IReadOnlyCollection<BaseDomainEvent> DomainEvents { get; }
}