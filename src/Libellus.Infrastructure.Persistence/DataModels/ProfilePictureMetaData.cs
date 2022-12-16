#pragma warning disable CS8618

using Libellus.Infrastructure.Persistence.DataModels.Common;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class ProfilePictureMetaData : BaseImageMetaData<long, Guid>
{
    public ProfilePictureMetaData()
    {
    }

    public ProfilePictureMetaData(long id, Guid publicId, int width, int height, string mimeType, int dataSize,
        string objectName, ZonedDateTime createdOnUtc) : base(id, publicId, width, height, mimeType, dataSize,
        objectName, createdOnUtc)
    {
    }
}