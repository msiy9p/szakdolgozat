using NodaTime;

namespace Libellus.Domain.Common.Interfaces.Services;

public interface IDateTimeProvider
{
    DateTimeZone TimeZone { get; }
    Instant Now { get; }
    LocalDateTime LocalNow { get; }
    ZonedDateTime UtcNow { get; }

    Instant? ToInstant(LocalDateTime? local);

    LocalDateTime? ToLocal(Instant? instant);

    ZonedDateTime? ToZonedDateTimeUtc(DateTime? dateTime);
    ZonedDateTime? ToZonedDateTimeUtc(DateOnly? dateOnly);
}