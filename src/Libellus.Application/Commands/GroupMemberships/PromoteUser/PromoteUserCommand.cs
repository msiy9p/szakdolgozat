using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.GroupMemberships.PromoteUser;

public sealed record PromoteUserCommand(UserId UserId) : ICommand;