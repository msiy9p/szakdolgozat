using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class GenreIdConverter : ValueConverter<GenreId, Guid>
{
    public GenreIdConverter() :
        base(id => id.Value,
            guid => new GenreId(guid))
    {
    }
}