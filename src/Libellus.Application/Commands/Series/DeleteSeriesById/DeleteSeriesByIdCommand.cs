using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Series.DeleteSeriesById;

public sealed record DeleteSeriesByIdCommand(SeriesId SeriesId) : ICommand;