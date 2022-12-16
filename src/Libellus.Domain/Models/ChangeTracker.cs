using Libellus.Domain.Common.Interfaces.Models;

namespace Libellus.Domain.Models;

public sealed class ChangeTracker<TItem> : IChangeTracker<TItem> where TItem : IEquatable<TItem>
{
    private readonly Dictionary<TItem, int> _newItems = new();
    private readonly Dictionary<TItem, int> _removedItems = new();
    private readonly Stepper _newStepper = new();
    private readonly Stepper _removedStepper = new();

    public bool HasChanges => _newItems.Count > 0 || _removedItems.Count > 0;

    public ChangeTracker()
    {
    }

    public void Add(TItem item)
    {
        if (item is null)
        {
            return;
        }

        _newItems.Add(item, _newStepper.Step());
        _removedItems.Remove(item);
    }

    public void Remove(TItem item)
    {
        if (item is null)
        {
            return;
        }

        _removedItems.Add(item, _removedStepper.Step());
        _newItems.Remove(item);
    }

    public void Clear()
    {
        _newItems.Clear();
        _removedItems.Clear();
    }

    public IReadOnlySet<TItem> GetNewItems() => _newItems.Keys.ToHashSet();

    public IReadOnlySet<TItem> GetRemovedItems() => _removedItems.Keys.ToHashSet();

    public IReadOnlyCollection<TItem> GetNewItemsChronologically() =>
        _newItems.OrderBy(x => x.Value).Select(x => x.Key).ToList();

    public IReadOnlyCollection<TItem> GetRemovedItemsChronologically() =>
        _removedItems.OrderBy(x => x.Value).Select(x => x.Key).ToList();

    private sealed class Stepper
    {
        private int _current;

        public Stepper()
        {
            _current = -1;
        }

        public int Step() => _current++;
    }
}