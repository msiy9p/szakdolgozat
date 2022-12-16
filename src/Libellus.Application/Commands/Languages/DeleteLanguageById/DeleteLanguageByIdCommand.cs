using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Languages.DeleteLanguageById;

public sealed record DeleteLanguageByIdCommand(LanguageId LanguageId) : ICommand;