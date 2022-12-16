using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.LiteratureForms.DeleteLiteratureFormById;

public sealed record DeleteLiteratureFormByIdCommand(LiteratureFormId LiteratureFormId) : ICommand;