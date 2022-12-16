using Libellus.Application.Common.Interfaces.Persistence.Specifications;
using Libellus.Application.Common.Models;
using Libellus.Application.Models;

namespace Libellus.Application.Specifications;

public class PaginatedSpecification<T> : BaseSpecification<T>, IPaginatedSpecification<T>
{
    public PaginationInfo PaginationInfo { get; init; }

    public PaginatedSpecification(PaginationInfo paginationInfo)
    {
        PaginationInfo = paginationInfo;
    }

    public override bool IsSatisfiedBy(T entity) => true;
}