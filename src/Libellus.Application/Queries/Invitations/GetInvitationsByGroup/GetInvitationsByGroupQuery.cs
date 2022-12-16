using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Enums;

namespace Libellus.Application.Queries.Invitations.GetInvitationsByGroup;

[Authorise]
public sealed record GetInvitationsByGroupQuery
    (GroupId GroupId, InvitationStatus InvitationStatus, SortOrder SortOrder) : IQuery<ICollection<Invitation>>;