using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Labels.UpdateLabel;

public sealed record UpdateLabelCommand(Label Item) : ICommand;