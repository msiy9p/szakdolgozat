using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Labels.CreateLabel;

public sealed record CreateLabelCommand(ShortName Name) : ICommand<LabelId>;