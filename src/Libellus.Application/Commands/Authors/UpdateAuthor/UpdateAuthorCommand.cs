using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Authors.UpdateAuthor;

public sealed record UpdateAuthorCommand(Author Item) : ICommand;