using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Groups.GetGroupByNamePaginated;

public sealed class
    GetGroupByNamePaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetGroupByNamePaginatedQuery>
{
    public override void BuildPolicy(GetGroupByNamePaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}