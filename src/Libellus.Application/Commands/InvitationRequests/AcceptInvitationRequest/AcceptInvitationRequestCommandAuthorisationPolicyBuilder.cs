using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.InvitationRequests.AcceptInvitationRequest;

public sealed class AcceptInvitationRequestCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<AcceptInvitationRequestCommand>
{
    public override void BuildPolicy(AcceptInvitationRequestCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}