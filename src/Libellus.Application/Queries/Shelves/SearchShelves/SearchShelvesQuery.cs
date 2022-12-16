using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Shelves.SearchShelves;

public sealed record SearchShelvesQuery(SearchTerm SearchTerm, SortOrder SortOrder) : IQuery<ICollection<Shelf>>;