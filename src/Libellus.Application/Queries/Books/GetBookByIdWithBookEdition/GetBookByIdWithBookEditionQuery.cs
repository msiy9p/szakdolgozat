using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Books.GetBookByIdWithBookEdition;

public sealed record GetBookByIdWithBookEditionQuery(BookId BookId, SortOrder SortOrder) : IQuery<Book>;