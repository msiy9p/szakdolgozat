using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetPostByIdWithComments;

public sealed class
    GetPostByIdWithCommentsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetPostByIdWithCommentsQuery>
{
    public override void BuildPolicy(GetPostByIdWithCommentsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}