using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class PublisherIdConverter : ValueConverter<PublisherId, Guid>
{
    public PublisherIdConverter() :
        base(id => id.Value,
            guid => new PublisherId(guid))
    {
    }
}