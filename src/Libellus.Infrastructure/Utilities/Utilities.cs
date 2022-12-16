namespace Libellus.Infrastructure.Utilities;

internal static class Utilities
{
    public static float CalculateAspectRatio(int width, int height)
    {
        width = Math.Abs(width);
        height = Math.Abs(height);

        if (width <= height)
        {
            return (float)height / (float)width;
        }

        return (float)width / (float)height;
    }
}