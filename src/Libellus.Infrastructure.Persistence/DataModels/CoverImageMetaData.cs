#pragma warning disable CS8618

using Libellus.Infrastructure.Persistence.DataModels.Common;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class CoverImageMetaData : BaseImageMetaData<long, Guid>
{
    public CoverImageMetaData()
    {
    }

    public CoverImageMetaData(long id, Guid publicId, int width, int height, string mimeType, int dataSize,
        string objectName, ZonedDateTime createdOnUtc) : base(id, publicId, width, height, mimeType, dataSize,
        objectName, createdOnUtc)
    {
    }
}