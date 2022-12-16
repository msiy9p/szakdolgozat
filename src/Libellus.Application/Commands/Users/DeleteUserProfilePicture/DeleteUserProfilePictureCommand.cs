using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Users.DeleteUserProfilePicture;

public sealed record DeleteUserProfilePictureCommand(UserId UserId) : ICommand;