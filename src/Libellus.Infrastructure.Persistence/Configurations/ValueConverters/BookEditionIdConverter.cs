using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class BookEditionIdConverter : ValueConverter<BookEditionId, Guid>
{
    public BookEditionIdConverter() :
        base(id => id.Value,
            guid => new BookEditionId(guid))
    {
    }
}