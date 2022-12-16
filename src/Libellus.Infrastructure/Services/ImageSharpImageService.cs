using Ardalis.GuardClauses;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Enums;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Infrastructure.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Infrastructure.Services;

internal sealed class ImageSharpImageResizer : IImageResizerWithPreference
{
    private const int s_maxJpegQuality = 60;
    private const int s_minJpegQuality = 15;

    private const int s_maxWebpQuality = 60;
    private const int s_minWebpQuality = 15;

    private const float s_maxAllowedAspectRatio = 2.5f;

    private const ImageFormat s_preferredImageFormat = ImageFormat.Jpeg;
    private const ImageSizeWidth s_preferredImageWidth = ImageSizeWidth.Width1200;
    private const ImageSizeHeight s_preferredImageHeight = ImageSizeHeight.Height1200;

    private static readonly IReadOnlySet<ImageFormat> s_supportedImageFormats = new HashSet<ImageFormat>
        { ImageFormat.Jpeg, ImageFormat.Webp };

    private readonly ILogger<ImageSharpImageResizer> _logger;
    private readonly IDateTimeProvider _dateTime;

    public ImageFormat PreferredImageFormat => s_preferredImageFormat;
    public ImageSizeWidth PreferredImageWidth => s_preferredImageWidth;
    public ImageSizeHeight PreferredImageHeight => s_preferredImageHeight;
    public IReadOnlySet<ImageFormat> SupportedImageFormats => s_supportedImageFormats;

    public ImageSharpImageResizer(ILogger<ImageSharpImageResizer> logger, IDateTimeProvider dateTime)
    {
        _logger = Guard.Against.Null(logger);
        _dateTime = Guard.Against.Null(dateTime);
    }

    private static JpegEncoder GetJpegEncoder(double qualityMultiplier)
    {
        int q = Math.Clamp((int)Math.Round(s_maxJpegQuality * qualityMultiplier, 0), s_minJpegQuality,
            s_maxJpegQuality);

        return new JpegEncoder
        {
            Quality = q,
        };
    }

    private static WebpEncoder GetWebpEncoder(double qualityMultiplier)
    {
        int q = Math.Clamp((int)Math.Round(s_maxWebpQuality * qualityMultiplier, 0), s_minWebpQuality,
            s_maxWebpQuality);

        return new WebpEncoder
        {
            Quality = q,
            FileFormat = WebpFileFormatType.Lossy,
        };
    }

    private static IResampler GetDefaultResampler() => KnownResamplers.Lanczos5;

    public async Task<Result<IImageComplete>> ResizeAsync(IImageDataOnly image, ImageSizeWidth width,
        ImageFormat format, CancellationToken cancellationToken = default)
    {
        if (!ImageSizeWidthExtensions.IsDefined(width))
        {
            return ImageErrors.ImageSizeWidthNotSupported.ToErrorResult<IImageComplete>();
        }

        if (!ImageFormatExtensions.IsDefined(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<IImageComplete>();
        }

        if (!s_supportedImageFormats.Contains(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<IImageComplete>();
        }

        if (image is null || image.Data is null || image.Data.Length == 0)
        {
            return ImageErrors.ImageDataNotValid.ToErrorResult<IImageComplete>();
        }

        return await ResizeAsync(image, (int)width, 0, format, cancellationToken);
    }

    public async Task<Result<ICollection<IImageComplete>>> ResizeAsync(IImageDataOnly image,
        IEnumerable<ImageSizeWidth> widths, ImageFormat format, CancellationToken cancellationToken = default)
    {
        if (widths is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<IImageComplete>>();
        }

        var output = new List<IImageComplete>();
        foreach (var width in widths.ToHashSet())
        {
            if (!ImageSizeWidthExtensions.IsDefined(width))
            {
                return ImageErrors.ImageSizeWidthNotSupported.ToErrorResult<ICollection<IImageComplete>>();
            }

            var result = await ResizeAsync(image, (int)width, 0, format, cancellationToken);
            if (result.IsSuccess)
            {
                output.Add(result.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<IImageComplete>> ResizeToPreferredDefaultWidthAsync(IImageDataOnly image,
        CancellationToken cancellationToken = default) =>
        await ResizeAsync(image, s_preferredImageWidth, s_preferredImageFormat, cancellationToken);

    public async Task<Result<ICollection<IImageComplete>>> ResizeToAllWidthsAsync(IImageDataOnly image,
        ImageFormat format, CancellationToken cancellationToken = default)
    {
        if (!ImageFormatExtensions.IsDefined(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<ICollection<IImageComplete>>();
        }

        if (image is null || image.Data is null || image.Data.Length == 0)
        {
            return ImageErrors.ImageDataNotValid.ToErrorResult<ICollection<IImageComplete>>();
        }

        var dimensions = ImageSizeWidthExtensions.GetAll().Select(x => ((int)x, 0)).ToList();
        return await ResizeWithWaterfallAsync(image, dimensions, format, cancellationToken);
    }

    public async Task<Result<IImageComplete>> ResizeAsync(IImageDataOnly image, ImageSizeHeight height,
        ImageFormat format, CancellationToken cancellationToken = default)
    {
        if (!ImageSizeHeightExtensions.IsDefined(height))
        {
            return ImageErrors.ImageSizeHeightNotSupported.ToErrorResult<IImageComplete>();
        }

        if (!ImageFormatExtensions.IsDefined(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<IImageComplete>();
        }

        if (!s_supportedImageFormats.Contains(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<IImageComplete>();
        }

        if (image is null || image.Data is null || image.Data.Length == 0)
        {
            return ImageErrors.ImageDataNotValid.ToErrorResult<IImageComplete>();
        }

        return await ResizeAsync(image, 0, (int)height, format, cancellationToken);
    }

    public async Task<Result<ICollection<IImageComplete>>> ResizeAsync(IImageDataOnly image,
        IEnumerable<ImageSizeHeight> heights, ImageFormat format, CancellationToken cancellationToken = default)
    {
        if (heights is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<IImageComplete>>();
        }

        var output = new List<IImageComplete>();
        foreach (var height in heights.ToHashSet())
        {
            if (!ImageSizeHeightExtensions.IsDefined(height))
            {
                return ImageErrors.ImageSizeWidthNotSupported.ToErrorResult<ICollection<IImageComplete>>();
            }

            var result = await ResizeAsync(image, 0, (int)height, format, cancellationToken);
            if (result.IsSuccess)
            {
                output.Add(result.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<IImageComplete>> ResizeToPreferredDefaultHeightAsync(IImageDataOnly image,
        CancellationToken cancellationToken = default) =>
        await ResizeAsync(image, s_preferredImageHeight, s_preferredImageFormat, cancellationToken);

    public async Task<Result<ICollection<IImageComplete>>> ResizeToAllHeightsAsync(IImageDataOnly image,
        ImageFormat format, CancellationToken cancellationToken = default)
    {
        if (!ImageFormatExtensions.IsDefined(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<ICollection<IImageComplete>>();
        }

        if (!s_supportedImageFormats.Contains(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<ICollection<IImageComplete>>();
        }

        if (image is null || image.Data is null || image.Data.Length == 0)
        {
            return ImageErrors.ImageDataNotValid.ToErrorResult<ICollection<IImageComplete>>();
        }

        var dimensions = ImageSizeHeightExtensions.GetAll().Select(x => (0, (int)x)).ToList();
        return await ResizeWithWaterfallAsync(image, dimensions, format, cancellationToken);
    }

    private async Task<Result<IImageComplete>> ResizeAsync(IImageDataOnly image, int width, int height,
        ImageFormat format, CancellationToken cancellationToken)
    {
        if (width < 0)
        {
            return GeneralErrors.InputIsNegative.ToErrorResult<IImageComplete>();
        }

        if (height < 0)
        {
            return GeneralErrors.InputIsNegative.ToErrorResult<IImageComplete>();
        }

        if (width == 0 && height == 0)
        {
            return ImageErrors.ImageSizeNotSupported.ToErrorResult<IImageComplete>();
        }

        if (!ImageFormatExtensions.IsDefined(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<IImageComplete>();
        }

        if (!s_supportedImageFormats.Contains(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<IImageComplete>();
        }

        if (image is null || image.Data is null || image.Data.Length == 0)
        {
            return ImageErrors.ImageDataNotValid.ToErrorResult<IImageComplete>();
        }

        using var stream = new MemoryStream(image.Data);
        using var img = await Image.LoadAsync(stream, cancellationToken);
        if (Utilities.Utilities.CalculateAspectRatio(img.Width, img.Height) > s_maxAllowedAspectRatio)
        {
            return ImageErrors.ImageAspectRatioNotSupported.ToErrorResult<IImageComplete>();
        }

        img.RemoveMetadata();

        img.Mutate(x => x.Resize(width, height, GetDefaultResampler()));

        using var outputStream = new MemoryStream();
        switch (format)
        {
            case ImageFormat.Jpeg:
                await img.SaveAsJpegAsync(outputStream, GetJpegEncoder(QualityMultiplier(width, height)),
                    cancellationToken);
                break;

            case ImageFormat.Webp:
                await img.SaveAsWebpAsync(outputStream, GetWebpEncoder(QualityMultiplier(width, height)),
                    cancellationToken);
                break;

            default:
                goto case s_preferredImageFormat;
        }

        return Result<IImageComplete>.Success(new ImageComplete(img.Width, img.Height, format, outputStream.ToArray(),
            _dateTime.UtcNow));
    }

    private async Task<Result<ICollection<IImageComplete>>> ResizeWithWaterfallAsync(IImageDataOnly image,
        IReadOnlyCollection<(int, int)> dimensions, ImageFormat format, CancellationToken cancellationToken)
    {
        if (dimensions is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<IImageComplete>>();
        }

        if (!ImageFormatExtensions.IsDefined(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<ICollection<IImageComplete>>();
        }

        if (!s_supportedImageFormats.Contains(format))
        {
            return ImageErrors.ImageFormatNotSupported.ToErrorResult<ICollection<IImageComplete>>();
        }

        if (image is null || image.Data is null || image.Data.Length == 0)
        {
            return ImageErrors.ImageDataNotValid.ToErrorResult<ICollection<IImageComplete>>();
        }

        using var stream = new MemoryStream(image.Data);
        using var img = await Image.LoadAsync(stream, cancellationToken);
        if (Utilities.Utilities.CalculateAspectRatio(img.Width, img.Height) > s_maxAllowedAspectRatio)
        {
            return ImageErrors.ImageAspectRatioNotSupported.ToErrorResult<ICollection<IImageComplete>>();
        }

        img.RemoveMetadata();

        var output = new List<IImageComplete>(dimensions.Count);
        foreach ((int width, int height) in dimensions)
        {
            if (width < 0)
            {
                _logger.LogError("{Width} is negative.", width);
                continue;
            }

            if (height < 0)
            {
                _logger.LogError("{Height} is negative.", height);
                continue;
            }

            if (width == 0 && height == 0)
            {
                _logger.LogError("{Width} and {Height}.", width, height);
                continue;
            }

            img.Mutate(x => x.Resize(width, height, GetDefaultResampler()));

            using var outputStream = new MemoryStream();
            switch (format)
            {
                case ImageFormat.Jpeg:
                    await img.SaveAsJpegAsync(outputStream, GetJpegEncoder(QualityMultiplier(width, height)),
                        cancellationToken);
                    break;

                case ImageFormat.Webp:
                    await img.SaveAsWebpAsync(outputStream, GetWebpEncoder(QualityMultiplier(width, height)),
                        cancellationToken);
                    break;

                default:
                    goto case s_preferredImageFormat;
            }

            output.Add(new ImageComplete(img.Width, img.Height, format, outputStream.ToArray(), _dateTime.UtcNow));
        }

        return output.ToResultCollection();
    }

    private static double QualityMultiplier(int width, int height)
    {
        if (width < 0)
        {
            return 1f;
        }

        if (height < 0)
        {
            return 1f;
        }

        if (width == 0 && height == 0)
        {
            return 1f;
        }

        double max;
        double current;

        if (height == 0)
        {
            max = Math.Sqrt((int)ImageSizeWidthExtensions.Biggest);
            current = Math.Sqrt(width);

            return current / max;
        }

        if (width == 0)
        {
            max = Math.Sqrt((int)ImageSizeHeightExtensions.Biggest);
            current = Math.Sqrt(height);

            return current / max;
        }

        max = Math.Sqrt(Math.Sqrt((int)ImageSizeWidthExtensions.Biggest) +
                        Math.Sqrt((int)ImageSizeHeightExtensions.Biggest));
        current = Math.Sqrt(Math.Sqrt(width) + Math.Sqrt(height));

        return current / max;
    }
}