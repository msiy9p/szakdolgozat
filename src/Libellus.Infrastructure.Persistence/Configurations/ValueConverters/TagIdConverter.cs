using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class TagIdConverter : ValueConverter<TagId, Guid>
{
    public TagIdConverter() :
        base(id => id.Value,
            guid => new TagId(guid))
    {
    }
}