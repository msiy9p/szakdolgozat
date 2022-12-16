using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Invitations.InviteUser;

public sealed class
    InviteUserCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<InviteUserCommand>
{
    public override void BuildPolicy(InviteUserCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
        UseRequirement(new NotInCurrentGroupRequirement(instance.InviteeId));
    }
}