using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.Users.TwoFactorSignIn;

// No Authorisation
public sealed record TwoFactorSignInCommand(string TwoFactorCode, bool RememberMe, bool RememberMachine) : ICommand;