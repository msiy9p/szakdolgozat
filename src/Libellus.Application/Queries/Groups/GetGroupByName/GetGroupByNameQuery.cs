using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Groups.GetGroupByName;

[Authorise]
public sealed record GetGroupByNameQuery(Name Name, SortOrder SortOrder) : IQuery<ICollection<Group>>;