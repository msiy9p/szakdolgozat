using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Labels.DeleteLabelById;

public sealed class
    DeleteLabelByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteLabelByIdCommand>
{
    public override void BuildPolicy(DeleteLabelByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}