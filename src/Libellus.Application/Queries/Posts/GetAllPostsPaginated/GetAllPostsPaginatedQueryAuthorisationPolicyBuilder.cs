using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetAllPostsPaginated;

public sealed class
    GetAllPostsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllPostsPaginatedQuery>
{
    public override void BuildPolicy(GetAllPostsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}