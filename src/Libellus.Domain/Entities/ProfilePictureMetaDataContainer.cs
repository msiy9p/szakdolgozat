using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Domain.Entities;

public sealed class
    ProfilePictureMetaDataContainer : BaseImageMetaDataContainer<ProfilePictureMetaData, ProfilePictureId>
{
    public ProfilePictureMetaDataContainer(ProfilePictureId id, IEnumerable<ProfilePictureMetaData> metaData) : base(id,
        metaData)
    {
    }
}