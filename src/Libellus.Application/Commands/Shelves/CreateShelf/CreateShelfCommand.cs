using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Shelves.CreateShelf;

public sealed record CreateShelfCommand
    (Name Name, DescriptionText? DescriptionText, bool IsLocked) : ICommand<ShelfIds>;