using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.GenerateUserEmailConfirmationToken;

// No Authorisation
public sealed record GenerateUserEmailConfirmationTokenCommand(UserId UserId,
    EmailConfirmationCallbackUrlTemplate EmailConfirmationCallbackUrlTemplate) : ICommand;