using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.GroupMemberships.PromoteModerator;

public sealed class
    PromoteModeratorCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        PromoteModeratorCommand>
{
    public override void BuildPolicy(PromoteModeratorCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupOwnerRequirement.Instance);
        UseRequirement(new GroupModeratorRequirement(instance.UserId));
    }
}