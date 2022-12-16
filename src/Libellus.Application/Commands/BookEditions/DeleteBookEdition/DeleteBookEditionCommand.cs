using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.BookEditions.DeleteBookEdition;

public sealed record DeleteBookEditionCommand(BookEdition Item) : ICommand;