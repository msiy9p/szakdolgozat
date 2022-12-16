using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.ResetUserPassword;

public sealed record ResetUserPasswordCommand(UserId UserId, string ResetToken, string NewPassword) : ICommand;