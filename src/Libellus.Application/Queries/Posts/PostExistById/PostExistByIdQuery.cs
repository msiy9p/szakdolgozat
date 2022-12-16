using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Posts.PostExistById;

public sealed record PostExistByIdQuery(PostId PostId) : IQuery<bool>;