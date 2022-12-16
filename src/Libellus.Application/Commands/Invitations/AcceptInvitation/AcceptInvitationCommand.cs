using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Invitations.AcceptInvitation;

[Authorise]
public sealed record AcceptInvitationCommand(InvitationId InvitationId) : ICommand;