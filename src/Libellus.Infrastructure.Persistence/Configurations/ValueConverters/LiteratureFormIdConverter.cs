using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class LiteratureFormIdConverter : ValueConverter<LiteratureFormId, Guid>
{
    public LiteratureFormIdConverter() :
        base(id => id.Value,
            guid => new LiteratureFormId(guid))
    {
    }
}