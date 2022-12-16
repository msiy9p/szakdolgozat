using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Tags.GetTagById;

public sealed record GetTagByIdQuery(TagId TagId) : IQuery<Tag>;