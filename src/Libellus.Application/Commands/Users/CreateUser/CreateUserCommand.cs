using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.CreateUser;

// No Authorisation
public sealed record CreateUserCommand
(string Email, string UserName, string Password, bool CreateWithDefaults,
    EmailConfirmationCallbackUrlTemplate CallbackUrlTemplate) : ICommand<UserId>;