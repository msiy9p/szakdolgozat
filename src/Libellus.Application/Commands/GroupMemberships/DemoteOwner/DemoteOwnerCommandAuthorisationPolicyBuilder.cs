using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.GroupMemberships.DemoteOwner;

public sealed class
    DemoteOwnerCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DemoteOwnerCommand>
{
    public override void BuildPolicy(DemoteOwnerCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupOwnerRequirement.Instance);
        UseRequirement(new GroupOwnerRequirement(instance.UserId));
    }
}