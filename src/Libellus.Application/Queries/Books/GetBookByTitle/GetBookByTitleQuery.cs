using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Books.GetBookByTitle;

public sealed record GetBookByTitleQuery(Title Title, SortOrder SortOrder) : IQuery<ICollection<Book>>;