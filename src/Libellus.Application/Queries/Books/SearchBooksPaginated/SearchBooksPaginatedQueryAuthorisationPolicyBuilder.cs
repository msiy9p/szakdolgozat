using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.SearchBooksPaginated;

public sealed class
    SearchBooksPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchBooksPaginatedQuery>
{
    public override void BuildPolicy(SearchBooksPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}