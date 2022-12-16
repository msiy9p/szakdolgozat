using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.LiteratureForms.CreateLiteratureForm;

public sealed class
    CreateLiteratureFormCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        CreateLiteratureFormCommand>
{
    public override void BuildPolicy(CreateLiteratureFormCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}