using NodaTime;

namespace Libellus.Domain.Utilities;

internal static class Utilities
{
    public static bool DoesPrecede(ZonedDateTime a, ZonedDateTime b)
    {
        return ZonedDateTime.Comparer.Instant.Compare(a, b) <= 0;
    }
}