using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Authors.UpdateAuthor;

public sealed class
    UpdateAuthorCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateAuthorCommand>
{
    public override void BuildPolicy(UpdateAuthorCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}