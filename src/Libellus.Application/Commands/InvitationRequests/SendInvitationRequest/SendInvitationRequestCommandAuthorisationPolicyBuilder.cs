using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.InvitationRequests.SendInvitationRequest;

public sealed class
    SendInvitationRequestCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        SendInvitationRequestCommand>
{
    private readonly ICurrentUserService _currentUserService;

    public SendInvitationRequestCommandAuthorisationPolicyBuilder(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override void BuildPolicy(SendInvitationRequestCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
        UseRequirement(new NotInCurrentGroupRequirement(_currentUserService.UserId!.Value));
    }
}