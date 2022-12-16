using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Readings.CreateReading;

public sealed record CreateReadingCommand(BookEditionId BookEditionId, bool IsDnf, bool IsReread,
    DateOnly? StartedOnUtc, DateOnly? FinishedOnUtc) : ICommand<ReadingIds>;