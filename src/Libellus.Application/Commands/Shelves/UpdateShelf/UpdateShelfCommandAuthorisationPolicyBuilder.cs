using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.UpdateShelf;

public sealed class
    UpdateShelfCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateShelfCommand>
{
    public override void BuildPolicy(UpdateShelfCommand instance)
    {
        UseRequirement(new CurrentUserCanEditShelfRequirement(instance.Item.Id));
    }
}