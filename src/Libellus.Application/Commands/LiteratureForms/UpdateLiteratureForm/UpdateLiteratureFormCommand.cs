using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.LiteratureForms.UpdateLiteratureForm;

public sealed record UpdateLiteratureFormCommand(LiteratureForm Item) : ICommand;