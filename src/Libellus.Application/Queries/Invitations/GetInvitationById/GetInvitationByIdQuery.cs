using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Invitations.GetInvitationById;

[Authorise]
public sealed record GetInvitationByIdQuery(InvitationId InvitationId) : IQuery<Invitation>;