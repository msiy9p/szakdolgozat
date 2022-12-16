using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Formats.DeleteFormatById;

public sealed class
    DeleteFormatByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteFormatByIdCommand>
{
    public override void BuildPolicy(DeleteFormatByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}