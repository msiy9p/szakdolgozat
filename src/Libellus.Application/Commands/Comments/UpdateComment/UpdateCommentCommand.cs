using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Comments.UpdateComment;

public sealed record UpdateCommentCommand(Comment Item) : ICommand;