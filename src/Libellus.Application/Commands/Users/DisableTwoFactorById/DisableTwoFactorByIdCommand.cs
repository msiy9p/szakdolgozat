using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.DisableTwoFactorById;

public sealed record DisableTwoFactorByIdCommand(UserId UserId) : ICommand;