using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class UserIdConverter : ValueConverter<UserId, Guid>
{
    public UserIdConverter() :
        base(id => id.Value,
            guid => new UserId(guid))
    {
    }
}