using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Shelves.GetAllShelvesByBookId;

public sealed record GetAllShelvesByBookIdQuery(BookId BookId, bool Containing, SortOrder SortOrder) :
    IQuery<ICollection<Shelf>>;