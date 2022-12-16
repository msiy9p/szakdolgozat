using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.DeleteShelf;

public sealed class
    DeleteShelfCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteShelfCommand>
{
    public override void BuildPolicy(DeleteShelfCommand instance)
    {
        UseRequirement(new CurrentUserCanDeleteShelfRequirement(instance.Item.Id));
    }
}