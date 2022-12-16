namespace Libellus.Domain.Common.Interfaces.Models;

public interface IReadOnlyChangeTracker<TItem> where TItem : IEquatable<TItem>
{
    bool HasChanges { get; }

    IReadOnlySet<TItem> GetNewItems();

    IReadOnlySet<TItem> GetRemovedItems();

    IReadOnlyCollection<TItem> GetNewItemsChronologically();

    IReadOnlyCollection<TItem> GetRemovedItemsChronologically();
}

public interface IChangeTracker<TItem> : IReadOnlyChangeTracker<TItem> where TItem : IEquatable<TItem>
{
    void Add(TItem item);

    void Remove(TItem item);

    void Clear();
}