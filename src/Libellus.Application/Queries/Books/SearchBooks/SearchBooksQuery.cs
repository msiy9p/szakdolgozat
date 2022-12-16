using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Books.SearchBooks;

public sealed record SearchBooksQuery(SearchTerm SearchTerm, SortOrder SortOrder) : IQuery<ICollection<Book>>;