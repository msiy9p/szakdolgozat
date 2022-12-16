using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.BookEditions.DeleteBookEdition;

public sealed class
    DeleteBookEditionCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteBookEditionCommand>
{
    public override void BuildPolicy(DeleteBookEditionCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}