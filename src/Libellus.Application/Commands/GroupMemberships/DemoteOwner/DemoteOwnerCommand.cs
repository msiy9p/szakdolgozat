using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.GroupMemberships.DemoteOwner;

public sealed record DemoteOwnerCommand(UserId UserId) : ICommand;