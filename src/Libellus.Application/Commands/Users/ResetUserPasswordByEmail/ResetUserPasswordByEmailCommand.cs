using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.Users.ResetUserPasswordByEmail;

// No Authorisation
public sealed record ResetUserPasswordByEmailCommand(string Email, string ResetToken, string NewPassword) : ICommand;