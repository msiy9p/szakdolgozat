using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class BookIdConverter : ValueConverter<BookId, Guid>
{
    public BookIdConverter() :
        base(id => id.Value,
            guid => new BookId(guid))
    {
    }
}