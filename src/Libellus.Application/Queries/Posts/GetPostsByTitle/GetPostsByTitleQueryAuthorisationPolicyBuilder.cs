using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetPostsByTitle;

public sealed class
    GetPostsByTitleQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetPostsByTitleQuery>
{
    public override void BuildPolicy(GetPostsByTitleQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}