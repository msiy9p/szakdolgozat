using Libellus.Application.Common.Interfaces.Persistence.Specifications;

namespace Libellus.Application.Common.Models;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification()
    {
    }

    public abstract bool IsSatisfiedBy(T entity);
}