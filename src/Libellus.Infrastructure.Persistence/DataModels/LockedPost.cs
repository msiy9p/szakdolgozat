using Libellus.Domain.Common.Types.Ids;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class LockedPost
{
    public PostId PostId { get; set; }
    public UserId? LockCreatorId { get; set; }
    public string? LockReason { get; set; }
    public ZonedDateTime CreatedOnUtc { get; set; }

    public Post Post { get; set; }
    public ApplicationUser? LockCreator { get; set; }

    public LockedPost()
    {
    }

    public LockedPost(PostId postId, UserId? lockCreatorId, string? lockReason, ZonedDateTime createdOnUtc)
    {
        PostId = postId;
        LockCreatorId = lockCreatorId;
        LockReason = lockReason;
        CreatedOnUtc = createdOnUtc;
    }
}