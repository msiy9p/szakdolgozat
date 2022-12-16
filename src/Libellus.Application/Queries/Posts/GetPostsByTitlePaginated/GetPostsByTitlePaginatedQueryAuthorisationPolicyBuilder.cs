using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetPostsByTitlePaginated;

public sealed class
    GetPostsByTitlePaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetPostsByTitlePaginatedQuery>
{
    public override void BuildPolicy(GetPostsByTitlePaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}