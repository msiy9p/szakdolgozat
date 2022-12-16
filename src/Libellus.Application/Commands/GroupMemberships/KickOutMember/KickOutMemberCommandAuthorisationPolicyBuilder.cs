using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.GroupMemberships.KickOutMember;

public sealed class
    KickOutMemberCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<KickOutMemberCommand>
{
    public override void BuildPolicy(KickOutMemberCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
        UseRequirement(new GroupMemberRequirement(instance.UserId));
    }
}