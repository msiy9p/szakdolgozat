using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;

namespace Libellus.Infrastructure.Services;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    private readonly ILogger<DateTimeProvider> _logger;
    private readonly IClock _clock;

    public DateTimeZone TimeZone { get; init; }
    public Instant Now => _clock.GetCurrentInstant();
    public LocalDateTime LocalNow => Now.InZone(TimeZone).LocalDateTime;
    public ZonedDateTime UtcNow => Now.InUtc();

    public DateTimeProvider(ILogger<DateTimeProvider> logger) : this(logger, SystemClock.Instance)
    {
    }

    public DateTimeProvider(ILogger<DateTimeProvider> logger, IClock clock)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
        _clock = Guard.Against.Null(clock, nameof(clock));

        try
        {
            TimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }
        catch (Exception)
        {
            _logger.LogError("Could not get system default timezone using tzdb.");
            TimeZone = BclDateTimeZone.ForSystemDefault();
        }
    }

    public DateTimeProvider(ILogger<DateTimeProvider> logger, IClock clock, DateTimeZone dateTimeZone)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
        _clock = Guard.Against.Null(clock, nameof(clock));
        TimeZone = Guard.Against.Null(dateTimeZone, nameof(dateTimeZone));
    }

    public Instant? ToInstant(LocalDateTime? local) => local?.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

    public LocalDateTime? ToLocal(Instant? instant) => instant?.InZone(TimeZone).LocalDateTime;

    public ZonedDateTime? ToZonedDateTimeUtc(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        var localDate = LocalDateTime.FromDateTime(dateTime.Value);
        var temp = TimeZone.AtLeniently(localDate);
        return temp.WithZone(DateTimeZone.Utc);
    }

    public ZonedDateTime? ToZonedDateTimeUtc(DateOnly? dateOnly)
    {
        if (!dateOnly.HasValue)
        {
            return null;
        }

        var dateTime = dateOnly.Value.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(12)));

        return ToZonedDateTimeUtc(dateTime);
    }
}