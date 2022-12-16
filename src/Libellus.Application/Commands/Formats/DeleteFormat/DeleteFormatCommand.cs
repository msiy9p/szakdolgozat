using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Formats.DeleteFormat;

public sealed record DeleteFormatCommand(Format Item) : ICommand;