using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.GroupMemberships.DemoteModerator;

public sealed class
    DemoteModeratorCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DemoteModeratorCommand>
{
    public override void BuildPolicy(DemoteModeratorCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupOwnerRequirement.Instance);
        UseRequirement(new GroupModeratorRequirement(instance.UserId));
    }
}