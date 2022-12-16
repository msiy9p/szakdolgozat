using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Authors.SearchAuthorsPaginated;

public sealed class
    SearchAuthorsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchAuthorsPaginatedQuery>
{
    public override void BuildPolicy(SearchAuthorsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}