using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Labels.UpdateLabel;

public sealed class
    UpdateLabelCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateLabelCommand>
{
    public override void BuildPolicy(UpdateLabelCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}