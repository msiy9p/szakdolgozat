using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class PostIdConverter : ValueConverter<PostId, Guid>
{
    public PostIdConverter() :
        base(id => id.Value,
            guid => new PostId(guid))
    {
    }
}