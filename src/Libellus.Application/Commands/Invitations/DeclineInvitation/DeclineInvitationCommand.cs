using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Invitations.DeclineInvitation;

[Authorise]
public sealed record DeclineInvitationCommand(InvitationId InvitationId) : ICommand;