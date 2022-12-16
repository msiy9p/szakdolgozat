#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Comment : BaseStampedModel<CommentId, GroupId>
{
    public string FriendlyId { get; set; }
    public UserId CreatorId { get; set; }
    public PostId PostId { get; set; }

    public CommentId? RepliedToId { get; set; }

    public string Text { get; set; }

    public Comment? RepliedTo { get; set; }
    public ApplicationUser Creator { get; set; }
    public Post Post { get; set; }

    public Comment()
    {
    }

    public Comment(CommentId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        string friendlyId, UserId creatorId, PostId postId, string text, CommentId? repliedToId) : base(id, groupId,
        createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        PostId = postId;
        Text = text;
        RepliedToId = repliedToId;
    }
}