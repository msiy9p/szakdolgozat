using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Commands.Authors.UpdateAuthorCoverImageById;

public sealed record UpdateAuthorCoverImageByIdCommand(AuthorId AuthorId,
    CoverImageMetaDataContainer CoverImageMetaDataContainer) : ICommand;