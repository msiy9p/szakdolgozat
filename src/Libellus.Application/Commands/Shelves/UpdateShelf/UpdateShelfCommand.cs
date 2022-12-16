using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Shelves.UpdateShelf;

public sealed record UpdateShelfCommand(Shelf Item) : ICommand;