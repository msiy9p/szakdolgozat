using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Books.DeleteBookById;

public sealed record DeleteBookByIdCommand(BookId BookId) : ICommand;