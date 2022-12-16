using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Comments.GetAllComments;

public sealed class
    GetAllCommentsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllCommentsQuery>
{
    public override void BuildPolicy(GetAllCommentsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}