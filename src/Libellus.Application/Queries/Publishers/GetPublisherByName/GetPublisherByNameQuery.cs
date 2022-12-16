using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Publishers.GetPublisherByName;

public sealed record GetPublisherByNameQuery(ShortName Name) : IQuery<Publisher>;