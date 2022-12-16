using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Readings.UpdateReading;

public sealed record UpdateReadingCommand(Reading Item) : ICommand;