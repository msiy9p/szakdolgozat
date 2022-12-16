using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class ShelfIdConverter : ValueConverter<ShelfId, Guid>
{
    public ShelfIdConverter() :
        base(id => id.Value,
            guid => new ShelfId(guid))
    {
    }
}