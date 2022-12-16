using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Comments.DeleteCommentById;

public sealed record DeleteCommentByIdCommand(CommentId CommentId) : ICommand;