using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Tags.DeleteTag;

public sealed class
    DeleteTagCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteTagCommand>
{
    public override void BuildPolicy(DeleteTagCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}