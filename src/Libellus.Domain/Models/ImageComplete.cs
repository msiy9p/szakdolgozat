using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Enums;
using NodaTime;

namespace Libellus.Domain.Models;

public sealed class ImageComplete : IImageComplete
{
    public int Width { get; init; }
    public int Height { get; init; }
    public ImageFormat ImageFormat { get; init; }
    public int DataSize { get; init; }
    public byte[] Data { get; init; }
    public ZonedDateTime CreatedOnUtc { get; init; }

    public ImageComplete(int width, int height, ImageFormat imageFormat, byte[] data, ZonedDateTime createdOnUtc)
    {
        Width = Guard.Against.NegativeOrZero(width);
        Height = Guard.Against.NegativeOrZero(height);
        ImageFormat = Guard.Against.ImageFormatOutOfRange(imageFormat);
        Data = Guard.Against.Null(data);
        Guard.Against.Zero(Data.Length);
        DataSize = Data.Length;

        CreatedOnUtc = createdOnUtc;
    }
}