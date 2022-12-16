using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.ProfilePictures.GetProfilePictureByObjectName;

[Authorise]
public sealed record GetProfilePictureByObjectNameQuery(string ObjectName) : IQuery<ProfilePicture>;