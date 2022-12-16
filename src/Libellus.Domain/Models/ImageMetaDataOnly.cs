using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using NodaTime;

namespace Libellus.Domain.Models;

public sealed class ImageMetaDataOnly : IImageMetaData, IEquatable<ImageMetaDataOnly>
{
    public int Width { get; init; }
    public int Height { get; init; }
    public ImageFormat ImageFormat { get; init; }
    public int DataSize { get; init; }
    public ZonedDateTime CreatedOnUtc { get; init; }

    public ImageMetaDataOnly(int width, int height, ImageFormat imageFormat, int dataSize, ZonedDateTime createdOnUtc)
    {
        Width = Guard.Against.NegativeOrZero(width);
        Height = Guard.Against.NegativeOrZero(height);
        ImageFormat = Guard.Against.ImageFormatOutOfRange(imageFormat);
        DataSize = Guard.Against.NegativeOrZero(dataSize);

        CreatedOnUtc = createdOnUtc;
    }

    public static Result<ImageMetaDataOnly> Create(int width, int height, ImageFormat imageFormat, int dataSize,
        ZonedDateTime createdOnUtc)
    {
        if (width <= 0)
        {
            return Result<ImageMetaDataOnly>.Invalid(DomainErrors.ImageErrors.ImageSizeWidthNotSupported);
        }

        if (width <= 0)
        {
            return Result<ImageMetaDataOnly>.Invalid(DomainErrors.ImageErrors.ImageSizeHeightNotSupported);
        }

        if (!ImageFormatExtensions.IsDefined(imageFormat))
        {
            return Result<ImageMetaDataOnly>.Invalid(DomainErrors.ImageErrors.ImageFormatNotSupported);
        }

        if (width <= 0)
        {
            return Result<ImageMetaDataOnly>.Invalid(DomainErrors.ImageErrors.ImageDataNotValid);
        }

        return Result<ImageMetaDataOnly>.Success(new ImageMetaDataOnly(width, height, imageFormat, dataSize,
            createdOnUtc));
    }

    public bool Equals(ImageMetaDataOnly? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals((IImageMetaData)other);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ImageMetaDataOnly other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height, (int)ImageFormat, DataSize);
    }
}