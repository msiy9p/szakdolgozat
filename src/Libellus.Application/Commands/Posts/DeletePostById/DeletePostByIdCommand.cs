using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Posts.DeletePostById;

public sealed record DeletePostByIdCommand(PostId PostId) : ICommand;