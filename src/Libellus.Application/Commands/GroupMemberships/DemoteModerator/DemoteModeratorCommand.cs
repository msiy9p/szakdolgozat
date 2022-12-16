using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.GroupMemberships.DemoteModerator;

public sealed record DemoteModeratorCommand(UserId UserId) : ICommand;