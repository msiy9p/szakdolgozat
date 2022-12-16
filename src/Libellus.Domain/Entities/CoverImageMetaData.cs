using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class CoverImageMetaData : BaseImageMetaData<CoverImageId>
{
    private ObjectName? _objectName;

    internal CoverImageMetaData(CoverImageId id, int width, int height, ImageFormat imageFormat, int dataSize,
        ZonedDateTime createdOnUtc)
        : base(id, width, height, imageFormat, dataSize, createdOnUtc)
    {
    }

    public new static Result<CoverImageMetaData> Create(CoverImageId id, int width, int height, ImageFormat imageFormat,
        int dataSize, ZonedDateTime createdOnUtc)
    {
        var result = BaseImageMetaData<CoverImageId>.Create(id, width, height, imageFormat, dataSize, createdOnUtc);
        if (result.IsError)
        {
            return Result<CoverImageMetaData>.Invalid(result.Errors);
        }

        return Result<CoverImageMetaData>.Success(new CoverImageMetaData(id, width, height, imageFormat, dataSize,
            createdOnUtc));
    }

    public override ObjectName GetObjectName() => _objectName ??= new ObjectName(ToString());

    public override string ToString()
    {
        var result = ImageFormatExtensions.ToExtension(ImageFormat);

        if (result.IsSuccess)
        {
            return $"{Id:N}-{Width}w{result.Value}";
        }

        return $"{Id:N}-{Width}w";
    }
}