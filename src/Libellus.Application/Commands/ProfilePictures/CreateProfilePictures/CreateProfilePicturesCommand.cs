using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.ProfilePictures.CreateProfilePictures;

[Authorise]
public sealed record CreateProfilePicturesCommand(ImageDataOnly ImageDataOnly) :
    ICommand<ProfilePictureMetaDataContainer>;