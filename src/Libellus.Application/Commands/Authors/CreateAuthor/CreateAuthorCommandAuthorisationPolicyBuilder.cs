using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Authors.CreateAuthor;

public sealed class
    CreateAuthorCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateAuthorCommand>
{
    public override void BuildPolicy(CreateAuthorCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}