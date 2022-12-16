using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Languages.DeleteLanguage;

public sealed class
    DeleteLanguageCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteLanguageCommand>
{
    public override void BuildPolicy(DeleteLanguageCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}