using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Formats.UpdateFormat;

public sealed record UpdateFormatCommand(Format Item) : ICommand;