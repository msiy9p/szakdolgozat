using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.DeleteBookById;

public sealed class
    DeleteBookByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteBookByIdCommand>
{
    public override void BuildPolicy(DeleteBookByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}