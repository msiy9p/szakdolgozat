using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.InvitationRequests.SendInvitationRequest;

[Authorise]
public sealed record SendInvitationRequestCommand(GroupId GroupId) : ICommand;