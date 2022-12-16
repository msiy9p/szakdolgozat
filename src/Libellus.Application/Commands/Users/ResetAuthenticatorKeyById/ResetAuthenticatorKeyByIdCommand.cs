using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.ResetAuthenticatorKeyById;

public sealed record ResetAuthenticatorKeyByIdCommand(UserId UserId) : ICommand;