using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.CoverImages.DeleteCoverImagesById;

[Authorise]
public sealed record DeleteCoverImagesByIdCommand(CoverImageId CoverImageId) : ICommand;