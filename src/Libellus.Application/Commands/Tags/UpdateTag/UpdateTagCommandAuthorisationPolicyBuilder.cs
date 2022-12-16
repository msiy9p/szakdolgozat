using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Tags.UpdateTag;

public sealed class
    UpdateTagCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateTagCommand>
{
    public override void BuildPolicy(UpdateTagCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}