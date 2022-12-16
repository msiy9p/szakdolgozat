using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.WarningTags.DeleteWarningTag;

public sealed class
    DeleteWarningTagCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteWarningTagCommand>
{
    public override void BuildPolicy(DeleteWarningTagCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}