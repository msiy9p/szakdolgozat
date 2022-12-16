using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.LiteratureForms.UpdateLiteratureFormById;

public sealed record UpdateLiteratureFormByIdCommand(LiteratureFormId LiteratureFormId, ShortName Name) : ICommand;