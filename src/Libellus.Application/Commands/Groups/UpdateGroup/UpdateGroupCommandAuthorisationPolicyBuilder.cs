using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Groups.UpdateGroup;

public sealed class
    UpdateGroupCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateGroupCommand>
{
    public override void BuildPolicy(UpdateGroupCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}