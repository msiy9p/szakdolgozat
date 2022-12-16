using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Comments.CreateComment;

public sealed record CreateCommentCommand(PostId PostId, CommentText CommentText, CommentId? RepliedToCommentId) :
    ICommand<CommentIds>;