using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class LabelIdConverter : ValueConverter<LabelId, Guid>
{
    public LabelIdConverter() :
        base(id => id.Value,
            guid => new LabelId(guid))
    {
    }
}