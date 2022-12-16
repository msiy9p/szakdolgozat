using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Languages.CreateLanguage;

public sealed record CreateLanguageCommand(ShortName Name) : ICommand<LanguageId>;