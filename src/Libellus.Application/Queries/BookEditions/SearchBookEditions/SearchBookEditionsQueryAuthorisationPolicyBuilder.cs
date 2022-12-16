using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.SearchBookEditions;

public sealed class
    SearchBookEditionsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SearchBookEditionsQuery>
{
    public override void BuildPolicy(SearchBookEditionsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}