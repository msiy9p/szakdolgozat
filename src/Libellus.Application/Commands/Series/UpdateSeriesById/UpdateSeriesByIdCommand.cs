using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Series.UpdateSeriesById;

public sealed record UpdateSeriesByIdCommand(SeriesId SeriesId, Title Title) : ICommand;