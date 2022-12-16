using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Formats.DeleteFormatById;

public sealed record DeleteFormatByIdCommand(FormatId FormatId) : ICommand;