using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Posts.UpdatePost;

public sealed record UpdatePostCommand(Post Item) : ICommand;