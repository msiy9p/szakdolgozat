using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.ViewModels;

public sealed class InvitationPicturedVm
{
    public InvitationId InvitationId { get; init; }
    public UserPicturedVm Invitee { get; init; }
    public InvitationStatus InvitationStatus { get; init; }

    public InvitationPicturedVm(InvitationId invitationId, UserPicturedVm invitee, InvitationStatus invitationStatus)
    {
        InvitationId = invitationId;
        Invitee = Guard.Against.Null(invitee);
        InvitationStatus = invitationStatus;
    }
}