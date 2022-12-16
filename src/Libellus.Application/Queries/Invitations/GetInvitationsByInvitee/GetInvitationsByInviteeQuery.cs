using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;

namespace Libellus.Application.Queries.Invitations.GetInvitationsByInvitee;

[Authorise]
public sealed record GetInvitationsByInviteeQuery
    (UserId UserId, InvitationStatus InvitationStatus, SortOrder SortOrder) : IQuery<ICollection<Invitation>>;