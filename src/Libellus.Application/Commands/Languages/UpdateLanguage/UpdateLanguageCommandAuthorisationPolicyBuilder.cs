using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Languages.UpdateLanguage;

public sealed class
    UpdateLanguageCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateLanguageCommand>
{
    public override void BuildPolicy(UpdateLanguageCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}