using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Common.Models;

public abstract class BaseImageMetaData<TKey> : BaseEntity<TKey>, IImageMetaData where TKey : IEquatable<TKey>
{
    public int Width { get; set; }
    public int Height { get; set; }
    public ImageFormat ImageFormat { get; set; }
    public int DataSize { get; set; }
    public ZonedDateTime CreatedOnUtc { get; set; }

    protected BaseImageMetaData(TKey id, int width, int height, ImageFormat imageFormat, int dataSize,
        ZonedDateTime createdOnUtc) : base(id)
    {
        Width = Guard.Against.NegativeOrZero(width);
        Height = Guard.Against.NegativeOrZero(height);
        DataSize = Guard.Against.NegativeOrZero(dataSize);
        ImageFormat = Guard.Against.ImageFormatOutOfRange(imageFormat);

        CreatedOnUtc = createdOnUtc;
    }

    protected internal static Result Create(TKey id, int width, int height, ImageFormat imageFormat, int dataSize,
        ZonedDateTime createdOnUtc)
    {
        var result = Create(id);
        if (result.IsError)
        {
            return result;
        }

        if (width <= 0)
        {
            return Result.Invalid(DomainErrors.ImageErrors.ImageSizeWidthNotSupported);
        }

        if (height <= 0)
        {
            return Result.Invalid(DomainErrors.ImageErrors.ImageSizeHeightNotSupported);
        }

        if (dataSize <= 0)
        {
            return Result.Invalid(DomainErrors.ImageErrors.ImageDataNotValid);
        }

        if (!ImageFormatExtensions.IsDefined(imageFormat))
        {
            return Result.Invalid(DomainErrors.ImageErrors.ImageFormatNotSupported);
        }

        return Result.Success();
    }

    public abstract ObjectName GetObjectName();

    public override string ToString()
    {
        var result = ImageFormatExtensions.ToExtension(ImageFormat);

        if (result.IsSuccess)
        {
            return $"{Id}-{Width}w{result.Value}";
        }

        return $"{Id}-{Width}w";
    }
}