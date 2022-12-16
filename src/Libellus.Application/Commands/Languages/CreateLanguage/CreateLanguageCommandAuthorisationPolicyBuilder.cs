using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Languages.CreateLanguage;

public sealed class
    CreateLanguageCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateLanguageCommand>
{
    public override void BuildPolicy(CreateLanguageCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}