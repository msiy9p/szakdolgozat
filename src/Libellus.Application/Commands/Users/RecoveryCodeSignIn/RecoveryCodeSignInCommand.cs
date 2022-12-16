using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.Users.RecoveryCodeSignIn;

// No Authorisation
public sealed record RecoveryCodeSignInCommand(string RecoveryCode) : ICommand;