using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;

namespace Libellus.Application.Queries.Invitations.GetInvitationVmsByInvitee;

[Authorise]
public sealed record GetInvitationVmsByInviteeQuery
    (UserId UserId, InvitationStatus InvitationStatus, SortOrder SortOrder) : IQuery<ICollection<InvitationUserVm>>;