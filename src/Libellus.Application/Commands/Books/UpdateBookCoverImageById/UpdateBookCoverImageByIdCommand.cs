using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Books.UpdateBookCoverImageById;

public sealed record UpdateBookCoverImageByIdCommand(BookId BookId,
    CoverImageMetaDataContainer CoverImageMetaDataContainer) : ICommand;