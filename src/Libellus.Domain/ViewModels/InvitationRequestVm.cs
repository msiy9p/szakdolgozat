using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Domain.ViewModels;

public sealed class InvitationRequestVm
{
    public InvitationId InvitationId { get; init; }
    public GroupId GroupId { get; init; }
    public string UserName { get; init; }

    public InvitationRequestVm(InvitationId invitationId, GroupId groupId, string userName)
    {
        InvitationId = invitationId;
        GroupId = groupId;
        UserName = userName;
    }
}