using Libellus.Application.Common.Interfaces.Persistence.Specifications;
using Libellus.Application.Common.Models;

namespace Libellus.Application.Specifications;

public class OrderedSpecification<T> : BaseSpecification<T>
{
    private static readonly HashSet<string> _properties =
        typeof(T).GetProperties()
            .Where(x => x.CanRead)
            .Select(x => x.Name)
            .ToHashSet();

    private readonly List<IOrderedSpecification<T>> _order = new();

    public bool HasOrder => _order.Any();

    public bool HasPagination => PaginatedSpecification is not null;

    public IReadOnlyCollection<IOrderedSpecification<T>> Order => _order.AsReadOnly();

    public IPaginatedSpecification<T>? PaginatedSpecification { get; private set; }

    public OrderedSpecification()
    {
    }

    public override bool IsSatisfiedBy(T entity) => true;

    public bool AddOrder(IOrderedSpecification<T> order)
    {
        if (_properties.Contains(order.PropertyName) && _order.All(x => x.PropertyName != order.PropertyName))
        {
            _order.Add(order);
            return true;
        }

        return false;
    }

    public void UsePagination(IPaginatedSpecification<T> paginated)
    {
        PaginatedSpecification = paginated;
    }
}