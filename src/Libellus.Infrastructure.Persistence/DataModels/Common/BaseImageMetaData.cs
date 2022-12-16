#pragma warning disable CS8618

using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels.Common;

internal abstract class BaseImageMetaData<TKey, TPublicId> : IId<TKey>
    where TKey : IEquatable<TKey> where TPublicId : IEquatable<TPublicId>
{
    public TKey Id { get; set; }
    public TPublicId PublicId { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public string MimeType { get; set; }
    public int DataSize { get; set; }
    public string ObjectName { get; set; }
    public ZonedDateTime CreatedOnUtc { get; set; }

    protected BaseImageMetaData()
    {
    }

    protected BaseImageMetaData(TKey id, TPublicId publicId, int width, int height, string mimeType, int dataSize,
        string objectName, ZonedDateTime createdOnUtc)
    {
        Id = id;
        PublicId = publicId;
        Width = width;
        Height = height;
        MimeType = mimeType;
        DataSize = dataSize;
        ObjectName = objectName;
        CreatedOnUtc = createdOnUtc;
    }
}