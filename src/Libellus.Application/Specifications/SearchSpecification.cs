using Libellus.Application.Common.Interfaces.Persistence.Specifications;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Specifications;

public class SearchSpecification<T> : OrderedSpecification<T>, ISearchSpecification<T>
{
    public SearchTerm SearchTerm { get; init; }

    public SearchSpecification(SearchTerm searchTerm)
    {
        SearchTerm = searchTerm;
    }
}