using Libellus.Application.Enums;

namespace Libellus.Application.Common.Interfaces.Persistence.Specifications;

public interface IOrderedSpecification<in T> : ISpecification<T>
{
    string PropertyName { get; }
    SortOrder SortOrder { get; }
}