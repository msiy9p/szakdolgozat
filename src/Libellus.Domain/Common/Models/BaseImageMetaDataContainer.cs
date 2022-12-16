using Ardalis.GuardClauses;
using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Interfaces.Models;
using System.Collections;

namespace Libellus.Domain.Common.Models;

public abstract class BaseImageMetaDataContainer : IEntity
{
    private readonly List<BaseDomainEvent> _domainEvents = new();

    public bool HasDomainEvents => _domainEvents.Count > 0;
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseImageMetaDataContainer()
    {
    }

    protected bool AddDomainEvent(BaseDomainEvent domainEvent)
    {
        if (domainEvent is null)
        {
            return false;
        }

        _domainEvents.Add(domainEvent);

        return true;
    }

    protected bool RemoveDomainEvent(BaseDomainEvent domainEvent)
    {
        if (domainEvent is null)
        {
            return false;
        }

        _domainEvents.Remove(domainEvent);

        return true;
    }

    protected bool RemoveDomainEvent(Predicate<BaseDomainEvent> predicate)
    {
        if (predicate is null)
        {
            return false;
        }

        foreach (var baseDomainEvent in _domainEvents.FindAll(predicate))
        {
            _domainEvents.Remove(baseDomainEvent);
        }

        return true;
    }

    internal void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class BaseImageMetaDataContainer<TEntity, TKey> : BaseImageMetaDataContainer,
    IReadOnlyCollection<TEntity>,
    IEntity<TKey> where TEntity : IImageMetaData, IEntity<TKey> where TKey : IEquatable<TKey>
{
    private readonly List<TEntity> _entities = new();

    public TKey Id { get; init; }
    public int Count => _entities.Count;
    public IReadOnlyCollection<TEntity> AvailableImageMetaData => _entities.AsReadOnly();

    protected BaseImageMetaDataContainer(TKey id, IEnumerable<TEntity> metaData)
    {
        Guard.Against.Null(metaData);

        Id = id;

        foreach (var data in metaData)
        {
            if (metaData is not null)
            {
                _entities.Add(data);
            }
        }
    }

    public IEnumerator<TEntity> GetEnumerator() => _entities.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}