using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByTitlePaginated;

public sealed class
    GetBookEditionByTitlePaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetBookEditionByTitlePaginatedQuery>
{
    public override void BuildPolicy(GetBookEditionByTitlePaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}