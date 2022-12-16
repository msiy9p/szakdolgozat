using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Services;

public interface IImageResizerWithPreference : IImageResizer
{
    ImageFormat PreferredImageFormat { get; }
    ImageSizeWidth PreferredImageWidth { get; }
    ImageSizeHeight PreferredImageHeight { get; }

    Task<Result<IImageComplete>> ResizeToPreferredDefaultWidthAsync(IImageDataOnly image,
        CancellationToken cancellationToken = default);

    Task<Result<IImageComplete>> ResizeToPreferredDefaultHeightAsync(IImageDataOnly image,
        CancellationToken cancellationToken = default);
}