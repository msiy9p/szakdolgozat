using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Formats.CreateFormat;

public sealed record CreateFormatCommand(ShortName Name, bool IsDigital) : ICommand<FormatId>;