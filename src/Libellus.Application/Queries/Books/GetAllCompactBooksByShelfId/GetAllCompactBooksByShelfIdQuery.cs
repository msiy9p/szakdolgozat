using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Queries.Books.GetAllCompactBooksByShelfId;

public sealed record GetAllCompactBooksByShelfIdQuery(ShelfId ShelfId, SortOrder SortOrder) :
    IQuery<ICollection<BookCompactVm>>;