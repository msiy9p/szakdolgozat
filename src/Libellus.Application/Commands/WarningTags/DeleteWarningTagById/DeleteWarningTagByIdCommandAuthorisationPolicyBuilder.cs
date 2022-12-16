using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.WarningTags.DeleteWarningTagById;

public sealed class
    DeleteWarningTagByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteWarningTagByIdCommand>
{
    public override void BuildPolicy(DeleteWarningTagByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}