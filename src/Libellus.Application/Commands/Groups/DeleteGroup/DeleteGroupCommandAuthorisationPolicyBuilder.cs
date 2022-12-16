using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Groups.DeleteGroup;

public sealed class
    DeleteGroupCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteGroupCommand>
{
    public override void BuildPolicy(DeleteGroupCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupOwnerRequirement.Instance);
    }
}