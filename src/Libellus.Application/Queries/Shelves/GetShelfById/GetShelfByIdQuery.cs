using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Shelves.GetShelfById;

public sealed record GetShelfByIdQuery(ShelfId ShelfId) : IQuery<Shelf>;