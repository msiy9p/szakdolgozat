using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Books.DeleteBook;

public sealed record DeleteBookCommand(Book Item) : ICommand;