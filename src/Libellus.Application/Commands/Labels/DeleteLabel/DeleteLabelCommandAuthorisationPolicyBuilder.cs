using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Labels.DeleteLabel;

public sealed class
    DeleteLabelCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteLabelCommand>
{
    public override void BuildPolicy(DeleteLabelCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}