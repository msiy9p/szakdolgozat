using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Domain.Entities;

public sealed class CoverImageMetaDataContainer : BaseImageMetaDataContainer<CoverImageMetaData, CoverImageId>
{
    public CoverImageMetaDataContainer(CoverImageId id, IEnumerable<CoverImageMetaData> metaData) : base(id, metaData)
    {
    }
}