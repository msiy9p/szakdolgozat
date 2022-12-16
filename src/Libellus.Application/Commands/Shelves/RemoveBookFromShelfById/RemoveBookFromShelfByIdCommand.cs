using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Shelves.RemoveBookFromShelfById;

public sealed record RemoveBookFromShelfByIdCommand(ShelfId ShelfId, BookId BookId) : ICommand;