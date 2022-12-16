using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.AddBookToShelfById;

public sealed class AddBookToShelfByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<AddBookToShelfByIdCommand>
{
    public override void BuildPolicy(AddBookToShelfByIdCommand instance)
    {
        UseRequirement(new CurrentUserCanEditShelfRequirement(instance.ShelfId));
    }
}