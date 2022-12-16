using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.GenerateNewTwoFactorRecoveryCodes;

public sealed record GenerateNewTwoFactorRecoveryCodesCommand(UserId UserId) : ICommand<IReadOnlyCollection<string>>;