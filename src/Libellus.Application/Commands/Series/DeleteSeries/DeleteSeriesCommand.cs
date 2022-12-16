using Libellus.Application.Common.Interfaces.Messaging;

namespace Libellus.Application.Commands.Series.DeleteSeries;

public sealed record DeleteSeriesCommand(Domain.Entities.Series Item) : ICommand;