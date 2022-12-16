using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Invitations.InviteUser;

public sealed record InviteUserCommand(UserId InviteeId) : ICommand<InvitationId>;