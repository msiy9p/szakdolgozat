using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Labels.DeleteLabel;

public sealed record DeleteLabelCommand(Label Item) : ICommand;