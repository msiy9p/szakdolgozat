using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class AuthorIdConverter : ValueConverter<AuthorId, Guid>
{
    public AuthorIdConverter() :
        base(id => id.Value,
            guid => new AuthorId(guid))
    {
    }
}