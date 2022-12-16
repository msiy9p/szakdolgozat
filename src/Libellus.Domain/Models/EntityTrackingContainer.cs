using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using System.Linq.Expressions;

namespace Libellus.Domain.Models;

public sealed class EntityTrackingContainer<TEntity, TId> : IEntityTrackingContainer<TEntity, TId>
    where TEntity : IEntity<TId> where TId : IEquatable<TId>
{
    private readonly ChangeTracker<TId> _changeTracker = new();
    private readonly HashSet<TId> _itemIds = new();
    private readonly List<TEntity> _items = new();

    public int Count => _items.Count;
    public bool HasChanges => _changeTracker.HasChanges;
    public bool IsTracking { get; init; }

    private EntityTrackingContainer(bool tracking)
    {
        IsTracking = tracking;
    }

    public EntityTrackingContainer() : this(true)
    {
    }

    private EntityTrackingContainer(IEnumerable<TEntity> entities, bool tracking) : this(tracking)
    {
        Guard.Against.Null(entities);

        foreach (var entity in entities)
        {
            if (entity is not null && _itemIds.Add(entity.Id))
            {
                _items.Add(entity);
            }
        }
    }

    public EntityTrackingContainer(IEnumerable<TEntity> entities) : this(entities, true)
    {
    }

    public static EntityTrackingContainer<TEntity, TId> CreateNonTracking()
    {
        return new EntityTrackingContainer<TEntity, TId>(false);
    }

    public static EntityTrackingContainer<TEntity, TId> CreateNonTracking(IEnumerable<TEntity> entities)
    {
        return new EntityTrackingContainer<TEntity, TId>(entities, false);
    }

    public IReadOnlyCollection<TEntity> GetItems() => _items.ToList();

    public IReadOnlySet<TId> GetNewIds() => _changeTracker.GetNewItems();

    public IReadOnlySet<TId> GetRemovedIds() => _changeTracker.GetRemovedItems();

    public IReadOnlyCollection<TId> GetNewItemsChronologically() => _changeTracker.GetNewItemsChronologically();

    public IReadOnlyCollection<TId> GetRemovedItemsChronologically() => _changeTracker.GetRemovedItemsChronologically();

    private void TrackerAdd(TId id)
    {
        if (!IsTracking)
        {
            return;
        }

        _changeTracker.Add(id);
    }

    private void TrackerRemove(TId id)
    {
        if (!IsTracking)
        {
            return;
        }

        _changeTracker.Remove(id);
    }

    public bool Add(TEntity entity)
    {
        if (entity is null)
        {
            return false;
        }

        if (!_itemIds.Add(entity.Id))
        {
            return false;
        }

        _items.Add(entity);

        TrackerAdd(entity.Id);

        return true;
    }

    public bool Remove(TEntity entity)
    {
        if (entity is null)
        {
            return false;
        }

        return Remove(entity.Id);
    }

    public bool Remove(TId id)
    {
        if (id is null)
        {
            return false;
        }

        var index = _items.FindIndex(x => x.Id.Equals(id));
        if (index == -1)
        {
            TrackerRemove(id);
            _itemIds.Remove(id);

            return true;
        }

        _items.RemoveAt(index);
        _itemIds.Remove(id);

        TrackerRemove(id);

        return true;
    }

    public bool Contains(TEntity item)
    {
        if (item is null)
        {
            return false;
        }

        return Contains(item.Id);
    }

    public bool Contains(TId id)
    {
        if (id is null)
        {
            return false;
        }

        return _itemIds.Contains(id);
    }

    public TEntity? GetById(TId id)
    {
        if (id is null)
        {
            return default(TEntity);
        }

        if (!Contains(id))
        {
            return default(TEntity);
        }

        return _items.FirstOrDefault(x => x.Id.Equals(id));
    }

    public TEntity? FirstOrDefault(Func<TEntity, bool> predicate) => _items.FirstOrDefault(predicate);

    public TEntity? SingleOrDefault(Func<TEntity, bool> predicate) => _items.SingleOrDefault(predicate);

    public TEntity? Find(Expression<Func<TEntity, bool>> predicate) =>
        _items.Find(new Predicate<TEntity>(predicate.Compile()));

    public ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate) =>
        _items.FindAll(new Predicate<TEntity>(predicate.Compile()));

    public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate) => _items.Where(predicate);

    public void ClearAll()
    {
        _items.Clear();
        _itemIds.Clear();

        ClearChanges();
    }

    public void ClearChanges()
    {
        _changeTracker.Clear();
    }
}