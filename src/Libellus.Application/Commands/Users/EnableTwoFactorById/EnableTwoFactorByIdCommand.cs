﻿using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.EnableTwoFactorById;

public sealed record EnableTwoFactorByIdCommand(UserId UserId) : ICommand<IReadOnlyCollection<string>>;