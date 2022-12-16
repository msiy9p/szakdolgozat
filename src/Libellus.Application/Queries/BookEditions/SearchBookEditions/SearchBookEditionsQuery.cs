using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.BookEditions.SearchBookEditions;

public sealed record SearchBookEditionsQuery(SearchTerm SearchTerm, SortOrder SortOrder) :
    IQuery<ICollection<BookEdition>>;