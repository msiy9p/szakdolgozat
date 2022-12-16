using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.WarningTags.DeleteWarningTagById;

public sealed record DeleteWarningTagByIdCommand(WarningTagId WarningTagId) : ICommand;