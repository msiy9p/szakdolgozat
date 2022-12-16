using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEditionCoverImageById;

public sealed record UpdateBookEditionCoverImageByIdCommand(BookEditionId BookEditionId,
    CoverImageMetaDataContainer CoverImageMetaDataContainer) : ICommand;