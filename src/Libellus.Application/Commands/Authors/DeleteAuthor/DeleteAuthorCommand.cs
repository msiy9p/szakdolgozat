using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Authors.DeleteAuthor;

public sealed record DeleteAuthorCommand(Author Item) : ICommand;