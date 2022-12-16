using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Formats.CreateFormat;

public sealed class
    CreateFormatCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateFormatCommand>
{
    public override void BuildPolicy(CreateFormatCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}