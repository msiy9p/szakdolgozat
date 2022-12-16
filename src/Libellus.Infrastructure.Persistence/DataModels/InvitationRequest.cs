#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities.Identity;
using Libellus.Domain.Enums;
using Libellus.Infrastructure.Persistence.DataModels.Common;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class InvitationRequest : BaseStampedModel<InvitationId, GroupId>
{
    public UserId RequesterId { get; set; }
    public InvitationStatus InvitationStatus { get; set; }

    public ApplicationUser Requester { get; set; }

    public InvitationRequest()
    {
    }
}