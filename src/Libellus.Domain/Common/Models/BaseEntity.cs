using Ardalis.GuardClauses;
using Libellus.Domain.Common.Events;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Common.Models;

public abstract class BaseEntity : IEntity
{
    private readonly List<BaseDomainEvent> _domainEvents = new();

    public bool HasDomainEvents => _domainEvents.Count > 0;
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
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

public abstract class BaseEntity<TKey> : BaseEntity, IEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; init; }

    protected BaseEntity(TKey id)
    {
        Id = Guard.Against.Null(id);
    }

    protected internal static Result Create(TKey id)
    {
        return id is null ? GeneralErrors.InputIsNull.ToErrorResult() : Result.Success();
    }
}