using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Shelves.UpdateShelfById;

public sealed record UpdateShelfByIdCommand
    (ShelfId ShelfId, DescriptionText? DescriptionText, bool IsLocked) : ICommand;