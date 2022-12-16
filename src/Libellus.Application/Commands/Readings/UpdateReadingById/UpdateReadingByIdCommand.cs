using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Readings.UpdateReadingById;

public sealed record UpdateReadingByIdCommand(ReadingId ReadingId, bool IsDnf, bool IsReread,
    DateOnly? StartedOnUtc, DateOnly? FinishedOnUtc, CommentText? CommentText) : ICommand;