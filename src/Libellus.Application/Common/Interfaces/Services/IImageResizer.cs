using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Services;

public interface IImageResizer
{
    IReadOnlySet<ImageFormat> SupportedImageFormats { get; }

    Task<Result<IImageComplete>> ResizeAsync(IImageDataOnly image, ImageSizeWidth width, ImageFormat format,
        CancellationToken cancellationToken = default);

    Task<Result<IImageComplete>> ResizeAsync(IImageDataOnly image, ImageSizeHeight height, ImageFormat format,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<IImageComplete>>> ResizeAsync(IImageDataOnly image, IEnumerable<ImageSizeHeight> heights,
        ImageFormat format, CancellationToken cancellationToken = default);

    Task<Result<ICollection<IImageComplete>>> ResizeAsync(IImageDataOnly image, IEnumerable<ImageSizeWidth> widths,
        ImageFormat format, CancellationToken cancellationToken = default);

    Task<Result<ICollection<IImageComplete>>> ResizeToAllWidthsAsync(IImageDataOnly image, ImageFormat format,
        CancellationToken cancellationToken = default);

    Task<Result<ICollection<IImageComplete>>> ResizeToAllHeightsAsync(IImageDataOnly image, ImageFormat format,
        CancellationToken cancellationToken = default);
}