using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.ViewModels;

public class InvitationUserVm
{
    public InvitationId InvitationId { get; init; }
    public Name GroupName { get; init; }
    public InvitationStatus InvitationStatus { get; init; }

    public InvitationUserVm(InvitationId invitationId, Name groupName, InvitationStatus invitationStatus)
    {
        InvitationId = invitationId;
        GroupName = Guard.Against.Null(groupName);
        InvitationStatus = invitationStatus;
    }
}