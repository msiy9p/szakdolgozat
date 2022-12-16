using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Labels.UpdateLabelById;

public sealed record UpdateLabelByIdCommand(LabelId LabelId, ShortName Name) : ICommand;