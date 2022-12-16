using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Readings.DeleteReading;

public sealed record DeleteReadingCommand(Reading Item) : ICommand;