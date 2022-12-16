using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.GenerateUserPasswordResetToken;

// No Authorisation
public sealed record GenerateUserPasswordResetTokenCommand(UserId UserId,
    ForgotPasswordCallbackUrlTemplate ForgotPasswordCallbackUrlTemplate) : ICommand;