using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class ProfilePictureMetaData : BaseImageMetaData<ProfilePictureId>
{
    private ObjectName? _objectName;

    internal ProfilePictureMetaData(ProfilePictureId id, int width, int height, ImageFormat imageFormat, int dataSize,
        ZonedDateTime createdOnUtc)
        : base(id, width, height, imageFormat, dataSize, createdOnUtc)
    {
    }

    public new static Result<ProfilePictureMetaData> Create(ProfilePictureId id, int width, int height,
        ImageFormat imageFormat, int dataSize, ZonedDateTime createdOnUtc)
    {
        var result = BaseImageMetaData<ProfilePictureId>.Create(id, width, height, imageFormat, dataSize, createdOnUtc);
        if (result.IsError)
        {
            return Result<ProfilePictureMetaData>.Invalid(result.Errors);
        }

        return Result<ProfilePictureMetaData>.Success(new ProfilePictureMetaData(id, width, height, imageFormat,
            dataSize, createdOnUtc));
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