using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Posts.PostExistById;

public sealed class
    PostExistByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<PostExistByIdQuery>
{
    public override void BuildPolicy(PostExistByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}