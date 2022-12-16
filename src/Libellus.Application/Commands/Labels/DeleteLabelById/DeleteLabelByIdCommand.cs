using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Labels.DeleteLabelById;

public sealed record DeleteLabelByIdCommand(LabelId LabelId) : ICommand;