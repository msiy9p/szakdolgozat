using Ardalis.GuardClauses;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json;

namespace Libellus.Domain.JsonConverters;

public static class DomainJsonSerializerOptions
{
    public static JsonSerializerOptions GetOptions(bool indent = false)
    {
        return GetDefaultOptions(indent)
            .AddIdJsonConverters()
            .AddDomainEventJsonConverter()
            .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }

    private static JsonSerializerOptions GetDefaultOptions(bool indent)
    {
        var options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            WriteIndented = indent,
        };

        return options;
    }

    private static JsonSerializerOptions AddIdJsonConverters(this JsonSerializerOptions options)
    {
        Guard.Against.Null(options);

        // TODO: Add converters

        return options;
    }

    private static JsonSerializerOptions AddDomainEventJsonConverter(this JsonSerializerOptions options)
    {
        Guard.Against.Null(options);

        options.Converters.Add(new DomainEventJsonConverter());

        return options;
    }
}