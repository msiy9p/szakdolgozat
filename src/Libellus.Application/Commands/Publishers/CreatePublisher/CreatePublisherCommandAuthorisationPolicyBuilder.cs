using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Publishers.CreatePublisher;

public sealed class
    CreatePublisherCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreatePublisherCommand>
{
    public override void BuildPolicy(CreatePublisherCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}