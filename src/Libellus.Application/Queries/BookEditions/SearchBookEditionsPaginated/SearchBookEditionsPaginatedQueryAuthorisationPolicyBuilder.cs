using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.SearchBookEditionsPaginated;

public sealed class
    SearchBookEditionsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchBookEditionsPaginatedQuery>
{
    public override void BuildPolicy(SearchBookEditionsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}