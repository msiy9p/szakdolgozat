using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Publishers.UpdatePublisher;

public sealed class
    UpdatePublisherCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdatePublisherCommand>
{
    public override void BuildPolicy(UpdatePublisherCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}