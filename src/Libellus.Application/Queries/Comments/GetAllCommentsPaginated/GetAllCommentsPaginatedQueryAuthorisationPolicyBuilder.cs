using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Comments.GetAllCommentsPaginated;

public sealed class
    GetAllCommentsPaginatedQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllCommentsPaginatedQuery>
{
    public override void BuildPolicy(GetAllCommentsPaginatedQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}