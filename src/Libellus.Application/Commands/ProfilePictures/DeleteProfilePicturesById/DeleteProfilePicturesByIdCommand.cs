using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.ProfilePictures.DeleteProfilePicturesById;

[Authorise]
public sealed record DeleteProfilePicturesByIdCommand(ProfilePictureId ProfilePictureId) : ICommand;