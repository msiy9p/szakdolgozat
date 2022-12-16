using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ConfigurationOptions;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.DefaultEntities;

public static class DefaultPublishers
{
    private static bool _configured = false;
    private static readonly HashSet<ShortName> _items = new();

    public static bool HasDefaults => _configured && _items.Count > 0;

    internal static void ConfigureDefaults(IEnumerable<PublisherItemOption> items)
    {
        if (_configured)
        {
            return;
        }

        foreach (var itemOption in items)
        {
            if (ShortName.IsValidShortName(itemOption.Name))
            {
                _items.Add(new ShortName(itemOption.Name));
            }
        }

        _configured = true;
    }

    public static IReadOnlyCollection<Publisher> Create(UserId? userId, IDateTimeProvider dateTimeProvider)
    {
        if (!_configured || _items.Count < 1 || dateTimeProvider is null)
        {
            return Array.Empty<Publisher>();
        }

        var items = new List<Publisher>(_items.Count);
        foreach (var item in _items)
        {
            var dateTime = dateTimeProvider.UtcNow;
            items.Add(new Publisher(PublisherId.Create(), dateTime, dateTime, userId, item));
        }

        return items;
    }
}