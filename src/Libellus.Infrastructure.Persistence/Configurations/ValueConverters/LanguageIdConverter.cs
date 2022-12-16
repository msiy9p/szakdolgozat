using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class LanguageIdConverter : ValueConverter<LanguageId, Guid>
{
    public LanguageIdConverter() :
        base(id => id.Value,
            guid => new LanguageId(guid))
    {
    }
}