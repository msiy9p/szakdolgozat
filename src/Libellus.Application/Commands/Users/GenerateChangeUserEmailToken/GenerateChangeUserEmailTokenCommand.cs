using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.GenerateChangeUserEmailToken;

public sealed record GenerateChangeUserEmailTokenCommand(UserId UserId, string NewEmail,
    EmailConfirmationCallbackUrlTemplate EmailConfirmationCallbackUrlTemplate) : ICommand;