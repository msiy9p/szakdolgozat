using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Formats.UpdateFormatById;

public sealed record UpdateFormatByIdCommand(FormatId FormatId, ShortName Name) : ICommand;