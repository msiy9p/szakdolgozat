using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Tags.GetTagByName;

public sealed record GetTagByNameQuery(ShortName Name) : IQuery<Tag>;