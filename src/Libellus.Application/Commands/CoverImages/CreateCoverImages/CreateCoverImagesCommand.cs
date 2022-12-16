using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.CoverImages.CreateCoverImages;

[Authorise]
public sealed record CreateCoverImagesCommand(ImageDataOnly ImageDataOnly) : ICommand<CoverImageMetaDataContainer>;