using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Books.GetAllBooksByAuthorId;

public sealed record GetAllBooksByAuthorIdQuery(AuthorId AuthorId, SortOrder SortOrder) : IQuery<ICollection<Book>>;