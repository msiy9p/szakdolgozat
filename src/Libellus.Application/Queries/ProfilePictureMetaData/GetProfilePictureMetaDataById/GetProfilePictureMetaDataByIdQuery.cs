using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.ProfilePictureMetaData.GetProfilePictureMetaDataById;

[Authorise]
public sealed record GetProfilePictureMetaDataByIdQuery(ProfilePictureId ProfilePictureId) :
    IQuery<ProfilePictureMetaDataContainer>;