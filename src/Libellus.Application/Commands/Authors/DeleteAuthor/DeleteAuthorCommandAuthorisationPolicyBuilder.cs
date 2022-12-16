using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Authors.DeleteAuthor;

public sealed class
    DeleteAuthorCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteAuthorCommand>
{
    public override void BuildPolicy(DeleteAuthorCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}