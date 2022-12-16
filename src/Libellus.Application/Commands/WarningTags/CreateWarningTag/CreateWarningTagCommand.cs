using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.WarningTags.CreateWarningTag;

public sealed record CreateWarningTagCommand(ShortName Name) : ICommand<WarningTagId>;