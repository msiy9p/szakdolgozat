using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class WarningTagIdConverter : ValueConverter<WarningTagId, Guid>
{
    public WarningTagIdConverter() :
        base(id => id.Value,
            guid => new WarningTagId(guid))
    {
    }
}