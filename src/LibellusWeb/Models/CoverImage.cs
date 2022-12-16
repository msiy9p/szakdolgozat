using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using LibellusWeb.Common;

namespace LibellusWeb.Models;

public sealed class CoverImage :
    BaseImgSrcSetWriter<CoverImageMetaDataContainer, CoverImageMetaData, CoverImageId>
{
    public int SizeWidth { get; init; }

    public CoverImage(string linkBase, CoverImageMetaDataContainer? metaDataContainer, int sizeWidth = 100) :
        base(linkBase, metaDataContainer)
    {
        SizeWidth = sizeWidth;
    }

    public override string GetAltText() => "Cover image.";

    public override string GetSizes()
    {
        return "(min-width: 800px) 640px, 800px";
    }
}