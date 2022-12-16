using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.GetPostByIdWithCommentsPaginated;

public sealed class GetPostByIdWithCommentsPaginatedQueryAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GetPostByIdWithCommentsPaginatedQuery>
{
    public override void BuildPolicy(GetPostByIdWithCommentsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}