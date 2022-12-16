using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Books.UpdateBook;

public sealed record UpdateBookCommand(Book Item) : ICommand;