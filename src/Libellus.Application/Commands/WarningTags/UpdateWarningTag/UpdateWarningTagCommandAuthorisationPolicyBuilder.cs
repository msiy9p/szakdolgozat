using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.WarningTags.UpdateWarningTag;

public sealed class
    UpdateWarningTagCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        UpdateWarningTagCommand>
{
    public override void BuildPolicy(UpdateWarningTagCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}