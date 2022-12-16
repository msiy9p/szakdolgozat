using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ConfigurationOptions;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.DefaultEntities;

public static class DefaultFormats
{
    private static bool _configured = false;
    private static readonly Dictionary<ShortName, bool> _items = new();

    public static bool HasDefaults => _configured && _items.Count > 0;

    internal static void ConfigureDefaults(IEnumerable<FormatItemOption> items)
    {
        if (_configured)
        {
            return;
        }

        foreach (var itemOption in items)
        {
            if (ShortName.IsValidShortName(itemOption.Name))
            {
                _items.Add(new ShortName(itemOption.Name), itemOption.IsDigital);
            }
        }

        _configured = true;
    }

    public static IReadOnlyCollection<Format> Create(UserId? userId, IDateTimeProvider dateTimeProvider)
    {
        if (!_configured || _items.Count < 1 || dateTimeProvider is null)
        {
            return Array.Empty<Format>();
        }

        var items = new List<Format>(_items.Count);
        foreach (var item in _items)
        {
            var dateTime = dateTimeProvider.UtcNow;
            items.Add(new Format(FormatId.Create(), dateTime, dateTime, userId, item.Key, item.Value));
        }

        return items;
    }
}