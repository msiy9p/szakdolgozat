namespace Libellus.Domain.Enums;

public static class ImageSizeWidthExtensions
{
    public static readonly ImageSizeWidth Biggest = (ImageSizeWidth)GetAll().Cast<int>().Max();
    public static readonly ImageSizeWidth Smallest = (ImageSizeWidth)GetAll().Cast<int>().Min();

    public static bool IsDefined(ImageSizeWidth imageSizeWidth) =>
        imageSizeWidth switch
        {
            ImageSizeWidth.Width40 => true,
            ImageSizeWidth.Width80 => true,
            ImageSizeWidth.Width160 => true,
            ImageSizeWidth.Width240 => true,
            ImageSizeWidth.Width320 => true,
            ImageSizeWidth.Width480 => true,
            ImageSizeWidth.Width640 => true,
            ImageSizeWidth.Width800 => true,
            ImageSizeWidth.Width960 => true,
            ImageSizeWidth.Width1200 => true,
            _ => false
        };

    public static IEnumerable<ImageSizeWidth> GetAll()
    {
        yield return ImageSizeWidth.Width1200;
        yield return ImageSizeWidth.Width960;
        yield return ImageSizeWidth.Width800;
        yield return ImageSizeWidth.Width640;
        yield return ImageSizeWidth.Width480;
        yield return ImageSizeWidth.Width320;
        yield return ImageSizeWidth.Width240;
        yield return ImageSizeWidth.Width160;
        yield return ImageSizeWidth.Width80;
        yield return ImageSizeWidth.Width40;
    }
}

public static class ImageSizeHeightExtensions
{
    public static readonly ImageSizeHeight Biggest = (ImageSizeHeight)GetAll().Cast<int>().Max();
    public static readonly ImageSizeHeight Smallest = (ImageSizeHeight)GetAll().Cast<int>().Min();

    public static IEnumerable<ImageSizeHeight> GetAll()
    {
        yield return ImageSizeHeight.Height1200;
        yield return ImageSizeHeight.Height960;
        yield return ImageSizeHeight.Height800;
        yield return ImageSizeHeight.Height640;
        yield return ImageSizeHeight.Height480;
        yield return ImageSizeHeight.Height320;
        yield return ImageSizeHeight.Height240;
        yield return ImageSizeHeight.Height160;
        yield return ImageSizeHeight.Height80;
        yield return ImageSizeHeight.Height40;
    }

    public static bool IsDefined(ImageSizeHeight imageSizeHeight) =>
        imageSizeHeight switch
        {
            ImageSizeHeight.Height40 => true,
            ImageSizeHeight.Height80 => true,
            ImageSizeHeight.Height160 => true,
            ImageSizeHeight.Height240 => true,
            ImageSizeHeight.Height320 => true,
            ImageSizeHeight.Height480 => true,
            ImageSizeHeight.Height640 => true,
            ImageSizeHeight.Height800 => true,
            ImageSizeHeight.Height960 => true,
            ImageSizeHeight.Height1200 => true,
            _ => false
        };
}