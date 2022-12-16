using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Authors.SearchAuthors;

public sealed record SearchAuthorsQuery(SearchTerm SearchTerm, SortOrder SortOrder) : IQuery<ICollection<Author>>;