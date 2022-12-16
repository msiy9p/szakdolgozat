using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Groups.DeleteGroupById;

public sealed class
    DeleteGroupByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteGroupByIdCommand>
{
    public override void BuildPolicy(DeleteGroupByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupOwnerRequirement.Instance);
    }
}