using Libellus.Application.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Specifications;

public interface IPaginatedSpecification<in T> : ISpecification<T>
{
    PaginationInfo PaginationInfo { get; }
}