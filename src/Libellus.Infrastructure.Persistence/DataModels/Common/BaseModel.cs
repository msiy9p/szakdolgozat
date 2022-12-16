#pragma warning disable CS8618

using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;

namespace Libellus.Infrastructure.Persistence.DataModels.Common;

internal abstract class BaseModel<TKey, TGroupId> : IId<TKey>
    where TKey : IEquatable<TKey> where TGroupId : IEquatable<TGroupId>
{
    public TKey Id { get; set; }
    public TGroupId GroupId { get; set; }

    public Group Group { get; set; }

    protected BaseModel()
    {
    }

    protected BaseModel(TKey id, TGroupId groupId)
    {
        Id = id;
        GroupId = groupId;
    }
}