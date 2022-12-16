using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionById;

public sealed record GetBookEditionByIdQuery(BookEditionId BookEditionId) : IQuery<BookEdition>;