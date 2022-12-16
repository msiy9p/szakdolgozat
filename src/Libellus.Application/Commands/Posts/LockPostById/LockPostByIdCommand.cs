using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Posts.LockPostById;

public sealed record LockPostByIdCommand(PostId PostId, CommentText CommentText) : ICommand;