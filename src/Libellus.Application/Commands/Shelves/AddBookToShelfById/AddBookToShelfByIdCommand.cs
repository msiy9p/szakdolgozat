using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Shelves.AddBookToShelfById;

public sealed record AddBookToShelfByIdCommand(ShelfId ShelfId, BookId BookId) : ICommand;