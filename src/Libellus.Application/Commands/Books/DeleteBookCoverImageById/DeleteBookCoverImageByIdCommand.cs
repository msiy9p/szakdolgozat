using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Books.DeleteBookCoverImageById;

public sealed record DeleteBookCoverImageByIdCommand(BookId BookId) : ICommand;