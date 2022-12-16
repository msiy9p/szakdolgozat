using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class ProfilePictureIdConverter : ValueConverter<ProfilePictureId, Guid>
{
    public ProfilePictureIdConverter() :
        base(id => id.Value,
            guid => new ProfilePictureId(guid))
    {
    }
}