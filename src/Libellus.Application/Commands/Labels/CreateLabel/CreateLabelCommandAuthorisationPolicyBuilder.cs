using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Labels.CreateLabel;

public sealed class
    CreateLabelCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateLabelCommand>
{
    public override void BuildPolicy(CreateLabelCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}