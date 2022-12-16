using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIdWithReadings;

public record GetBookEditionByIdWithReadingsQuery
    (BookEditionId BookEditionId, SortOrder SortOrder) : IQuery<BookEdition>;