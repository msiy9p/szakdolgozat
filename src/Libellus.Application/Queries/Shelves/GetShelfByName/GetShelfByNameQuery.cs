using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Shelves.GetShelfByName;

public sealed record GetShelfByNameQuery(Name Name) : IQuery<Shelf>;