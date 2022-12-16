using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Series.CreateSeries;

public sealed record CreateSeriesCommand(Title Title) : ICommand<SeriesIds>;