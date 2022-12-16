namespace Libellus.Application.Common.Interfaces.Persistence.Specifications;

public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T entity);
}