using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIsbnPaginated;

public sealed class GetBookEditionByIsbnPaginatedQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetBookEditionByIsbnPaginatedQuery>
{
    public override void BuildPolicy(GetBookEditionByIsbnPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}