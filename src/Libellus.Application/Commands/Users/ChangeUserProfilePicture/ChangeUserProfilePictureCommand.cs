using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.ChangeUserProfilePicture;

public sealed record ChangeUserProfilePictureCommand(UserId UserId,
    ProfilePictureId ProfilePictureId) : ICommand;