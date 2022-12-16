using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Publishers.DeletePublisher;

public sealed class
    DeletePublisherCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeletePublisherCommand>
{
    public override void BuildPolicy(DeletePublisherCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}