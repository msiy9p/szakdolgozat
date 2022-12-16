using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Comments.DeleteComment;

public sealed record DeleteCommentCommand(Comment Item) : ICommand;