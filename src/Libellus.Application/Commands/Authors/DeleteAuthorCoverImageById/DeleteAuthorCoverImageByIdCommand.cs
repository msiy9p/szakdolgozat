using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Authors.DeleteAuthorCoverImageById;

public sealed record DeleteAuthorCoverImageByIdCommand(AuthorId AuthorId) : ICommand;