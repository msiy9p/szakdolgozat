using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.BookEditions.CreateBookEdition;

public sealed class
    CreateBookEditionCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        CreateBookEditionCommand>
{
    public override void BuildPolicy(CreateBookEditionCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}