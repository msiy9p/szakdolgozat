using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Readings.DeleteReadingById;

public sealed record DeleteReadingByIdCommand(ReadingId ReadingId) : ICommand;