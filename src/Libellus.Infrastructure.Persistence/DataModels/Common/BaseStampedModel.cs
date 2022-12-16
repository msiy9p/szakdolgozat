using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels.Common;

internal abstract class BaseStampedModel<TKey, TGroupId> : BaseModel<TKey, TGroupId> where TKey : IEquatable<TKey>
    where TGroupId : IEquatable<TGroupId>
{
    public ZonedDateTime CreatedOnUtc { get; set; }
    public ZonedDateTime ModifiedOnUtc { get; set; }

    protected BaseStampedModel()
    {
    }

    protected BaseStampedModel(TKey id, TGroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc) :
        base(id, groupId)
    {
        CreatedOnUtc = createdOnUtc;
        ModifiedOnUtc = modifiedOnUtc;
    }
}