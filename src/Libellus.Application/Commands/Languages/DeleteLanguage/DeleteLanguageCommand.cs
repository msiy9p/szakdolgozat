using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Languages.DeleteLanguage;

public sealed record DeleteLanguageCommand(Language Item) : ICommand;