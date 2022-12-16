using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Comments.CommentExistById;

public sealed record CommentExistByIdQuery(CommentId CommentId) : IQuery<bool>;