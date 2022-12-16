using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Comments.CreateComment;

public sealed class
    CreateCommentCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateCommentCommand>
{
    public override void BuildPolicy(CreateCommentCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
        UseRequirement(new PostNotLockedRequirement(instance.PostId));
    }
}