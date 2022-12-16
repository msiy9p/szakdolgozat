using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.ChangeUserEmail;

// No Authorisation
public sealed record ChangeUserEmailCommand(UserId UserId, string NewEmail, string Token) : ICommand;