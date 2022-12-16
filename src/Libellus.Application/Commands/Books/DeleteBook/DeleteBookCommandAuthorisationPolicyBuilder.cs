using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.DeleteBook;

public sealed class
    DeleteBookCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteBookCommand>
{
    public override void BuildPolicy(DeleteBookCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}