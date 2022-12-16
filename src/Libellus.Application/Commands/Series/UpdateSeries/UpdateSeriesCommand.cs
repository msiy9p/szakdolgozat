using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.Series.UpdateSeries;

public sealed record UpdateSeriesCommand(Domain.Entities.Series Item) : ICommand;