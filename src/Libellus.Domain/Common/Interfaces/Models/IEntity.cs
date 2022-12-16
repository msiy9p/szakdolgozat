namespace Libellus.Domain.Common.Interfaces.Models;

public interface IEntity : IHasDomainEvents
{
}

public interface IEntity<TKey> : IEntity where TKey : IEquatable<TKey>
{
    TKey Id { get; init; }
}