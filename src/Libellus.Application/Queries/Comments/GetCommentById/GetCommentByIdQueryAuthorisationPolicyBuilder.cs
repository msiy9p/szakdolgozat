using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Comments.GetCommentById;

public sealed class
    GetCommentByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetCommentByIdQuery>
{
    public override void BuildPolicy(GetCommentByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}