using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetAllPosts;

public sealed class
    GetAllPostsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllPostsQuery>
{
    public override void BuildPolicy(GetAllPostsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}