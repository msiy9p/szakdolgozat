using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.LiteratureForms.DeleteLiteratureForm;

public sealed class
    DeleteLiteratureFormCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteLiteratureFormCommand>
{
    public override void BuildPolicy(DeleteLiteratureFormCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}