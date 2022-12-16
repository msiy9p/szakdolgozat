using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.ChangeUserPassword;

public sealed record ChangeUserPasswordCommand(UserId UserId, string OldPassword, string NewPassword) : ICommand;