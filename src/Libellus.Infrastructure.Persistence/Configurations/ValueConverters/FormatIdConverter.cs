using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class FormatIdConverter : ValueConverter<FormatId, Guid>
{
    public FormatIdConverter() :
        base(id => id.Value,
            guid => new FormatId(guid))
    {
    }
}