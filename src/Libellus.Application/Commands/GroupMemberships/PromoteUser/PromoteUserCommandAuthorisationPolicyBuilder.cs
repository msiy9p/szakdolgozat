using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.GroupMemberships.PromoteUser;

public sealed class
    PromoteUserCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<PromoteUserCommand>
{
    public override void BuildPolicy(PromoteUserCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
        UseRequirement(new InCurrentGroupRequirement(instance.UserId));
    }
}