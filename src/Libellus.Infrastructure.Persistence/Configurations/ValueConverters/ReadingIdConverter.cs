using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class ReadingIdConverter : ValueConverter<ReadingId, Guid>
{
    public ReadingIdConverter() :
        base(id => id.Value,
            guid => new ReadingId(guid))
    {
    }
}