using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Languages.UpdateLanguageById;

public sealed record UpdateLanguageByIdCommand(LanguageId LanguageId, ShortName Name) : ICommand;