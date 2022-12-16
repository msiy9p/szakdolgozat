using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetPostById;

public sealed class
    GetPostByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetPostByIdQuery>
{
    public override void BuildPolicy(GetPostByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}