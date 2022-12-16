using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Common.Interfaces.Persistence.Specifications;

public interface ISearchSpecification<in T> : ISpecification<T>
{
    SearchTerm SearchTerm { get; }
}