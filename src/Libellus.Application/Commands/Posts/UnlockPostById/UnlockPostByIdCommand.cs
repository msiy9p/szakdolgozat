using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Posts.UnlockPostById;

public sealed record UnlockPostByIdCommand(PostId PostId) : ICommand;