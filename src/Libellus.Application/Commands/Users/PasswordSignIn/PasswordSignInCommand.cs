using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.Users.PasswordSignIn;

// No Authorisation
public sealed record PasswordSignInCommand(string Email, string Password, bool RememberMe) : ICommand;