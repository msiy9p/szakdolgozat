using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.InvitationRequests.AcceptInvitationRequest;

public sealed record AcceptInvitationRequestCommand(InvitationId InvitationId) : ICommand;