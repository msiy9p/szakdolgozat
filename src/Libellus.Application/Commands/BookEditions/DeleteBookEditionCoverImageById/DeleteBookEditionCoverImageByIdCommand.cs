using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.BookEditions.DeleteBookEditionCoverImageById;

public sealed record DeleteBookEditionCoverImageByIdCommand(BookEditionId BookEditionId) : ICommand;