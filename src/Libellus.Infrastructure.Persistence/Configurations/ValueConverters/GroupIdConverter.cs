using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class GroupIdConverter : ValueConverter<GroupId, Guid>
{
    public GroupIdConverter() :
        base(id => id.Value,
            guid => new GroupId(guid))
    {
    }
}