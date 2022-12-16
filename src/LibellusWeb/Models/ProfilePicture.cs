using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using LibellusWeb.Common;

namespace LibellusWeb.Models;

public sealed class ProfilePicture :
    BaseImgSrcSetWriter<ProfilePictureMetaDataContainer, ProfilePictureMetaData, ProfilePictureId>
{
    public ProfilePicture(string linkBase, ProfilePictureMetaDataContainer? metaDataContainer) :
        base(linkBase, metaDataContainer)
    {
    }

    public override string GetAltText() => "User profile picture.";

    public override string GetSizes()
    {
        return "(min-width: 600px) 160px, 320px";
    }
}