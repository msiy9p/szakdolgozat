using Libellus.Application.Commands.InvitationRequests.AcceptInvitationRequest;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.InvitationRequests.DeclineInvitationRequest;

public sealed class DeclineInvitationRequestCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<AcceptInvitationRequestCommand>
{
    public override void BuildPolicy(AcceptInvitationRequestCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}