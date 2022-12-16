using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.InvitationRequests.GetInvitationRequestById;

[Authorise]
public sealed record GetInvitationRequestByIdQuery(InvitationId InvitationId) : IQuery<InvitationRequest>;