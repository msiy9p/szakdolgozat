using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.LockPostById;

public sealed class
    LockPostByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<LockPostByIdCommand>
{
    public override void BuildPolicy(LockPostByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}