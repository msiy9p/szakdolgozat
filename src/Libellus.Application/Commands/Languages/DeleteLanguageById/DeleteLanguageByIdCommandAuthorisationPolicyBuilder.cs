using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Languages.DeleteLanguageById;

public sealed class
    DeleteLanguageByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteLanguageByIdCommand>
{
    public override void BuildPolicy(DeleteLanguageByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}