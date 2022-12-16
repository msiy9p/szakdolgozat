using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByIsbn;

public sealed record GetBookEditionByIsbnQuery(Isbn Isbn, SortOrder SortOrder) : IQuery<ICollection<BookEdition>>;