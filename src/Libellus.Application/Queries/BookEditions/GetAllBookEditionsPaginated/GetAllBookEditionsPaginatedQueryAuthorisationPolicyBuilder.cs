using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.BookEditions.GetAllBookEditionsPaginated;

public sealed class
    GetAllBookEditionsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllBookEditionsPaginatedQuery>
{
    public override void BuildPolicy(GetAllBookEditionsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}