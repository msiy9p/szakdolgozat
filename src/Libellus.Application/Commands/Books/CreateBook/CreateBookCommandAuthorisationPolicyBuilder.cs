using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.CreateBook;

public sealed class
    CreateBookCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateBookCommand>
{
    public override void BuildPolicy(CreateBookCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}