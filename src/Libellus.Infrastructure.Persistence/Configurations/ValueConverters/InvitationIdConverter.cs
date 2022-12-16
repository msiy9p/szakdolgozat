using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class InvitationIdConverter : ValueConverter<InvitationId, Guid>
{
    public InvitationIdConverter() :
        base(id => id.Value,
            guid => new InvitationId(guid))
    {
    }
}