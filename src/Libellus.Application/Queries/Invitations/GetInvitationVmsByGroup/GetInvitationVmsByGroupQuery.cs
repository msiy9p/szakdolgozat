using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;

namespace Libellus.Application.Queries.Invitations.GetInvitationVmsByGroup;

[Authorise]
public sealed record GetInvitationVmsByGroupQuery(GroupId GroupId, InvitationStatus InvitationStatus,
    SortOrder SortOrder) : IQuery<ICollection<InvitationPicturedVm>>;