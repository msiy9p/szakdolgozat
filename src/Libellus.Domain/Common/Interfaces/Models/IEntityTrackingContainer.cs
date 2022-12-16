using System.Linq.Expressions;

namespace Libellus.Domain.Common.Interfaces.Models;

public interface IReadOnlyEntityTrackingContainer<TEntity, TId> where TEntity : IEntity<TId> where TId : IEquatable<TId>
{
    int Count { get; }
    bool IsTracking { get; }
    bool HasChanges { get; }

    bool Contains(TEntity item);

    bool Contains(TId id);

    TEntity? GetById(TId id);

    IReadOnlyCollection<TEntity> GetItems();

    IReadOnlySet<TId> GetNewIds();

    IReadOnlySet<TId> GetRemovedIds();

    IReadOnlyCollection<TId> GetNewItemsChronologically();

    IReadOnlyCollection<TId> GetRemovedItemsChronologically();

    TEntity? FirstOrDefault(Func<TEntity, bool> predicate);

    TEntity? SingleOrDefault(Func<TEntity, bool> predicate);

    TEntity? Find(Expression<Func<TEntity, bool>> predicate);

    ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

    IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);
}

public interface IEntityTrackingContainer<TEntity, TId> : IReadOnlyEntityTrackingContainer<TEntity, TId>
    where TEntity : IEntity<TId> where TId : IEquatable<TId>
{
    bool Add(TEntity entity);

    bool Remove(TEntity entity);

    bool Remove(TId id);

    void ClearAll();

    void ClearChanges();
}