using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Entities.Identity;

namespace Libellus.Domain.Models;

public sealed class DomainEventContainer : IHasDomainEvents
{
    private readonly List<BaseDomainEvent> _domainEvents = new();

    public bool HasDomainEvents => _domainEvents.Count > 0;
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public DomainEventContainer()
    {
    }

    public void ClearEvents() => _domainEvents.Clear();

    public void GatherAndClearEvents(BaseEntity entity)
    {
        if (entity is null || !entity.HasDomainEvents)
        {
            return;
        }

        foreach (var domainEvent in entity.DomainEvents)
        {
            if (domainEvent is null)
            {
                continue;
            }

            _domainEvents.Add(domainEvent);
        }

        entity.ClearDomainEvents();
    }

    public void GatherAndClearEvents(BaseImageMetaDataContainer imageMetaDataContainer)
    {
        if (imageMetaDataContainer is null || imageMetaDataContainer.HasDomainEvents)
        {
            return;
        }

        foreach (var domainEvent in imageMetaDataContainer.DomainEvents)
        {
            if (domainEvent is null)
            {
                continue;
            }

            _domainEvents.Add(domainEvent);
        }

        imageMetaDataContainer.ClearDomainEvents();
    }

    public void GatherAndClearEvents(User user)
    {
        if (user is null || user.HasDomainEvents)
        {
            return;
        }

        foreach (var domainEvent in user.DomainEvents)
        {
            if (domainEvent is null)
            {
                continue;
            }

            _domainEvents.Add(domainEvent);
        }

        user.ClearDomainEvents();
    }

    public void GatherEvents(IHasDomainEvents domainEvents)
    {
        if (domainEvents is null || domainEvents.HasDomainEvents)
        {
            return;
        }

        foreach (var domainEvent in domainEvents.DomainEvents)
        {
            if (domainEvent is null)
            {
                continue;
            }

            _domainEvents.Add(domainEvent);
        }
    }

    public void GatherEvent(BaseDomainEvent domainEvent)
    {
        if (domainEvent is null)
        {
            return;
        }

        _domainEvents.Add(domainEvent);
    }
}