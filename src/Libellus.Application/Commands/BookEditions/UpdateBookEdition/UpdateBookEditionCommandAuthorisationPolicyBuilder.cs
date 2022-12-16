using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEdition;

public sealed class
    UpdateBookEditionCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        UpdateBookEditionCommand>
{
    public override void BuildPolicy(UpdateBookEditionCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}