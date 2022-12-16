using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.InvitationRequests.DeclineInvitationRequest;

public sealed record DeclineInvitationRequestCommand(InvitationId InvitationId) : ICommand;