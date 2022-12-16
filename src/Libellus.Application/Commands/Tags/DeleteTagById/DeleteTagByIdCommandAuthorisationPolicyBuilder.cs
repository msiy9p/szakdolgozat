using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Tags.DeleteTagById;

public sealed class
    DeleteTagByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteTagByIdCommand>
{
    public override void BuildPolicy(DeleteTagByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}