using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Authors.GetAuthorByName;

public sealed record GetAuthorByNameQuery(Name Name, SortOrder SortOrder) : IQuery<ICollection<Author>>;