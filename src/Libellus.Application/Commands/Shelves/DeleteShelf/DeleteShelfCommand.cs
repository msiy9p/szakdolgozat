using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Shelves.DeleteShelf;

public sealed record DeleteShelfCommand(Shelf Item) : ICommand;