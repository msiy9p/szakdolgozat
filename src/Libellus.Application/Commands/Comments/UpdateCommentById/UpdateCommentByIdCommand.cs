using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Comments.UpdateCommentById;

public sealed record UpdateCommentByIdCommand(CommentId CommentId, CommentText CommentText) : ICommand;