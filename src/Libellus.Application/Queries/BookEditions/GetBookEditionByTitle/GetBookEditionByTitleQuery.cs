using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.BookEditions.GetBookEditionByTitle;

public sealed record GetBookEditionByTitleQuery(Title Title, SortOrder SortOrder) : IQuery<ICollection<BookEdition>>;