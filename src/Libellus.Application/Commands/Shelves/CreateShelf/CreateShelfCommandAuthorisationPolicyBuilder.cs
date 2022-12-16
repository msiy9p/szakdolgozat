using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.CreateShelf;

public sealed class
    CreateShelfCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateShelfCommand>
{
    public override void BuildPolicy(CreateShelfCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}