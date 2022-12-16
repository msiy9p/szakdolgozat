using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Languages.UpdateLanguage;

public sealed record UpdateLanguageCommand(Language Item) : ICommand;