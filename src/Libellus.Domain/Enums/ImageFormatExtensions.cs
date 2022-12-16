using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Enums;

public static class ImageFormatExtensions
{
    public static bool IsDefined(ImageFormat imageFormat) =>
        imageFormat switch
        {
            ImageFormat.Jpeg => true,
            ImageFormat.Webp => true,
            ImageFormat.Png => true,
            _ => false
        };

    public static Result<string> ToMimeType(ImageFormat imageFormat) => imageFormat switch
    {
        ImageFormat.Jpeg => Result<string>.Success(MimeTypes.Image.Jpeg),
        ImageFormat.Webp => Result<string>.Success(MimeTypes.Image.Webp),
        ImageFormat.Png => Result<string>.Success(MimeTypes.Image.Png),
        _ => Result<string>.Error(ImageErrors.ImageFormatNotSupported),
    };

    public static Result<ImageFormat> FromMimeType(string mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
        {
            Result<ImageFormat>.Error(GeneralErrors.StringNullOrWhiteSpace);
        }

        return mimeType!.ToLowerInvariant() switch
        {
            MimeTypes.Image.Jpeg => Result<ImageFormat>.Success(ImageFormat.Jpeg),
            MimeTypes.Image.Webp => Result<ImageFormat>.Success(ImageFormat.Webp),
            MimeTypes.Image.Png => Result<ImageFormat>.Success(ImageFormat.Png),
            _ => Result<ImageFormat>.Error(ImageErrors.MimeTypeNotRecognized),
        };
    }

    public static Result<string> ToExtension(ImageFormat imageFormat) => imageFormat switch
    {
        ImageFormat.Jpeg => Result<string>.Success(".jpeg"),
        ImageFormat.Webp => Result<string>.Success(".webp"),
        ImageFormat.Png => Result<string>.Success(".png"),
        _ => Result<string>.Error(ImageErrors.ImageFormatNotSupported),
    };

    public static Result<ImageFormat> FromExtension(string extension) =>
        extension.ToLowerInvariant().Replace(".", "") switch
        {
            "jpeg" => Result<ImageFormat>.Success(ImageFormat.Jpeg),
            "webp" => Result<ImageFormat>.Success(ImageFormat.Webp),
            "png" => Result<ImageFormat>.Success(ImageFormat.Png),
            _ => Result<ImageFormat>.Error(ImageErrors.ImageFormatNotSupported),
        };
}