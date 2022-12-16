using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Books.GetBookById;

public sealed record GetBookByIdQuery(BookId BookId) : IQuery<Book>;