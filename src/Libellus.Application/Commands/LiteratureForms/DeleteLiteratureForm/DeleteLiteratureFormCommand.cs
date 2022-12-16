using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.LiteratureForms.DeleteLiteratureForm;

public sealed record DeleteLiteratureFormCommand(LiteratureForm Item) : ICommand;