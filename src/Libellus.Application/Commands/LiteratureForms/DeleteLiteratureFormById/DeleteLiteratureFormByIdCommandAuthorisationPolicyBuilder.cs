using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.LiteratureForms.DeleteLiteratureFormById;

public sealed class
    DeleteLiteratureFormByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteLiteratureFormByIdCommand>
{
    public override void BuildPolicy(DeleteLiteratureFormByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}