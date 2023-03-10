using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class NoteIdConverter : ValueConverter<NoteId, Guid>
{
    public NoteIdConverter() :
        base(id => id.Value,
            guid => new NoteId(guid))
    {
    }
}