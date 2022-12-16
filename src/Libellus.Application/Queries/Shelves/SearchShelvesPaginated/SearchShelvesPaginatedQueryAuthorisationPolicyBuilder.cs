using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.SearchShelvesPaginated;

public sealed class
    SearchShelvesPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchShelvesPaginatedQuery>
{
    public override void BuildPolicy(SearchShelvesPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}