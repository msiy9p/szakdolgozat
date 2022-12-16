using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Posts.DeletePost;

public sealed record DeletePostCommand(Post Item) : ICommand;