using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Posts.CreatePost;

public sealed record CreatePostCommand(Title Title, CommentText CommentText, bool IsMemberOnly, bool IsSpoiler,
    ShortName? Label) : ICommand<PostIds>;