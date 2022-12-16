using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.RemoveBookFromShelfById;

public sealed class RemoveBookFromShelfByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<RemoveBookFromShelfByIdCommand>
{
    public override void BuildPolicy(RemoveBookFromShelfByIdCommand instance)
    {
        UseRequirement(new CurrentUserCanEditShelfRequirement(instance.ShelfId));
    }
}