using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.SearchPostsPaginated;

public sealed class
    SearchPostsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchPostsPaginatedQuery>
{
    public override void BuildPolicy(SearchPostsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}