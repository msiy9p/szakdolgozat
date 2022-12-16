using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.UnlockPostById;

public sealed class
    UnlockPostByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UnlockPostByIdCommand>
{
    public override void BuildPolicy(UnlockPostByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}