using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.SearchPosts;

public sealed class
    SearchPostsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<SearchPostsQuery>
{
    public override void BuildPolicy(SearchPostsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}