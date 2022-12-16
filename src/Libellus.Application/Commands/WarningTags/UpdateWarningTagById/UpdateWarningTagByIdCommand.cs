using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.WarningTags.UpdateWarningTagById;

public sealed record UpdateWarningTagByIdCommand(WarningTagId WarningTagId, ShortName Name) : ICommand;