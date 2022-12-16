using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Shelves.DeleteShelfById;

public sealed record DeleteShelfByIdCommand(ShelfId ShelfId) : ICommand;