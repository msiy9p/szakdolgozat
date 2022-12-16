using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Shelves.GetShelfByIdWithBooks;

public sealed record GetShelfByIdWithBooksQuery(ShelfId ShelfId, SortOrder SortOrder) : IQuery<Shelf>;