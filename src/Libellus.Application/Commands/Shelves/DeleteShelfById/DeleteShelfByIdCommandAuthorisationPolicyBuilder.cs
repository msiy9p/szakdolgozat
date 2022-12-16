using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Shelves.DeleteShelfById;

public sealed class
    DeleteShelfByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteShelfByIdCommand>
{
    public override void BuildPolicy(DeleteShelfByIdCommand instance)
    {
        UseRequirement(new CurrentUserCanDeleteShelfRequirement(instance.ShelfId));
    }
}