using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Formats.DeleteFormat;

public sealed class
    DeleteFormatCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteFormatCommand>
{
    public override void BuildPolicy(DeleteFormatCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}