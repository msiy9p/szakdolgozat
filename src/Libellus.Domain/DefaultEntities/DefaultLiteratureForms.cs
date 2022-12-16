using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ConfigurationOptions;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.DefaultEntities;

public static class DefaultLiteratureForms
{
    private static bool _configured = false;
    private static readonly Dictionary<ShortName, ScoreMultiplier> _items = new();

    public static bool HasDefaults => _configured && _items.Count > 0;

    internal static void ConfigureDefaults(IEnumerable<LiteratureFormItemOption> items)
    {
        if (_configured)
        {
            return;
        }

        foreach (var itemOption in items)
        {
            if (ShortName.IsValidShortName(itemOption.Name) &&
                ScoreMultiplier.IsScoreMultiplierValid(itemOption.ScoreMultiplier))
            {
                _items.Add(new ShortName(itemOption.Name), new ScoreMultiplier(itemOption.ScoreMultiplier));
            }
        }

        _configured = true;
    }

    public static IReadOnlyCollection<LiteratureForm> Create(UserId? userId, IDateTimeProvider dateTimeProvider)
    {
        if (!_configured || _items.Count < 1 || dateTimeProvider is null)
        {
            return Array.Empty<LiteratureForm>();
        }

        var items = new List<LiteratureForm>(_items.Count);
        foreach (var item in _items)
        {
            var dateTime = dateTimeProvider.UtcNow;
            items.Add(new LiteratureForm(LiteratureFormId.Create(), dateTime, dateTime, userId, item.Key, item.Value));
        }

        return items;
    }
}