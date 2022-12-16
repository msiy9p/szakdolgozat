using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Comments.CommentExistById;

public sealed class
    CommentExistByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CommentExistByIdQuery>
{
    public override void BuildPolicy(CommentExistByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}