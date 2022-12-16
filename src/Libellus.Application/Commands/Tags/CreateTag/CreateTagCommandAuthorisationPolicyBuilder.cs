using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Tags.CreateTag;

public sealed class
    CreateTagCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateTagCommand>
{
    public override void BuildPolicy(CreateTagCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}