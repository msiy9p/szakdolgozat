using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Comments.GetCommentById;

public sealed record GetCommentByIdQuery(CommentId CommentId) : IQuery<Comment>;