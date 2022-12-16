using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.BookEditions.DeleteBookEditionById;

public sealed record DeleteBookEditionByIdCommand(BookEditionId BookEditionId) : ICommand;