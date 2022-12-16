using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.LiteratureForms.UpdateLiteratureForm;

public sealed class
    UpdateLiteratureFormCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        UpdateLiteratureFormCommand>
{
    public override void BuildPolicy(UpdateLiteratureFormCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}