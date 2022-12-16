using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.SearchShelves;

public sealed class
    SearchShelvesQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<SearchShelvesQuery>
{
    public override void BuildPolicy(SearchShelvesQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}