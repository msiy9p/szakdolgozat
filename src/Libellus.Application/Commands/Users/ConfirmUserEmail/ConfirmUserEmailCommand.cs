using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.ConfirmUserEmail;

// No Authorisation
public sealed record ConfirmUserEmailCommand(UserId UserId, string EmailToken) : ICommand;