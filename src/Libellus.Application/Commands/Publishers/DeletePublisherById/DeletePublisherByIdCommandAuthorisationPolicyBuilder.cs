using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Publishers.DeletePublisherById;

public sealed class
    DeletePublisherByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeletePublisherByIdCommand>
{
    public override void BuildPolicy(DeletePublisherByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}