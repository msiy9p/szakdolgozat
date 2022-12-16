using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models;

namespace Libellus.Application.Commands.Users.GenerateUserPasswordResetTokenByEmail;

// No Authorisation
public sealed record GenerateUserPasswordResetTokenByEmailCommand(string Email,
    ForgotPasswordCallbackUrlTemplate ForgotPasswordCallbackUrlTemplate) : ICommand;