using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.UpdateShelfById;

public sealed class UpdateShelfByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateShelfByIdCommand>
{
    public override void BuildPolicy(UpdateShelfByIdCommand instance)
    {
        UseRequirement(new CurrentUserCanEditShelfRequirement(instance.ShelfId));
    }
}