using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Formats.UpdateFormat;

public sealed class
    UpdateFormatCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateFormatCommand>
{
    public override void BuildPolicy(UpdateFormatCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}