using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.DeleteUserById;

public sealed record DeleteUserByIdCommand(UserId UserId) : ICommand;