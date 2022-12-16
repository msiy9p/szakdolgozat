using SixLabors.ImageSharp;

namespace Libellus.Infrastructure.Utilities.Extensions;

internal static class ImageSharpExtension
{
    public static void RemoveMetadata(this IImage image)
    {
        if (image is null)
        {
            return;
        }

        if (image.Metadata is null)
        {
            return;
        }

        image.Metadata.ExifProfile = null;
        image.Metadata.XmpProfile = null;
        image.Metadata.IptcProfile = null;
    }
}