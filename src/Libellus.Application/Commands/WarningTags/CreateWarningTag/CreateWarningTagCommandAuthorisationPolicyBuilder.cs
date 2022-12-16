using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.WarningTags.CreateWarningTag;

public sealed class
    CreateWarningTagCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        CreateWarningTagCommand>
{
    public override void BuildPolicy(CreateWarningTagCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}