using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Shelves.GetAllShelves;

public sealed record GetAllShelvesQuery(SortOrder SortOrder) : IQuery<ICollection<Shelf>>
{
    public static readonly GetAllShelvesQuery DefaultInstance = new(SortOrder.Ascending);
}