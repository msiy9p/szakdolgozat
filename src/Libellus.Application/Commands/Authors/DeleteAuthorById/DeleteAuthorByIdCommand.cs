using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Authors.DeleteAuthorById;

public sealed record DeleteAuthorByIdCommand(AuthorId AuthorId) : ICommand;