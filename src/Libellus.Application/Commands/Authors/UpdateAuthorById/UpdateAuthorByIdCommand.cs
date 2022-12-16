using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Authors.UpdateAuthorById;

public sealed record UpdateAuthorByIdCommand(AuthorId AuthorId, Name Name) : ICommand;