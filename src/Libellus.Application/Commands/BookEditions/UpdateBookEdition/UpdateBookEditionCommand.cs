using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEdition;

public sealed record UpdateBookEditionCommand(BookEdition Item) : ICommand;