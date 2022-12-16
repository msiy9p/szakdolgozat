#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Infrastructure.Persistence.DataModels.Common;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Invitation : BaseStampedModel<InvitationId, GroupId>
{
    public UserId InviterId { get; set; }
    public UserId InviteeId { get; set; }
    public InvitationStatus InvitationStatus { get; set; }

    public ApplicationUser Inviter { get; set; }
    public ApplicationUser Invitee { get; set; }

    public Invitation()
    {
    }
}