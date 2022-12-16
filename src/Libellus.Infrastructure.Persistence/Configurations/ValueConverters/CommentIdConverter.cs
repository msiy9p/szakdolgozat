using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class CommentIdConverter : ValueConverter<CommentId, Guid>
{
    public CommentIdConverter() :
        base(id => id.Value,
            guid => new CommentId(guid))
    {
    }
}