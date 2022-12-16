using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Posts.UpdatePostById;

public sealed record UpdatePostByIdCommand(PostId PostId, CommentText CommentText,
    bool IsMemberOnly, bool IsSpoiler, ShortName? Label) : ICommand;