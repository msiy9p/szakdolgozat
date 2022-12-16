using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.GroupMemberships.LeaveCurrentGroup;

public sealed class LeaveCurrentGroupCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<LeaveCurrentGroupCommand>
{
    private readonly ICurrentUserService _currentUserService;

    public LeaveCurrentGroupCommandAuthorisationPolicyBuilder(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override void BuildPolicy(LeaveCurrentGroupCommand instance)
    {
        var userId = _currentUserService.UserId;

        UseRequirement(new InCurrentGroupRequirement(userId.Value));
    }
}