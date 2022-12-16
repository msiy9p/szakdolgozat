#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities.Identity;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;
using NodaTime;
using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Post : BaseStampedModel<PostId, GroupId>, ISearchable
{
    public string FriendlyId { get; set; }
    public UserId? CreatorId { get; set; }
    public LabelId? LabelId { get; set; }

    public string Title { get; set; }
    public string TitleNormalized { get; set; }
    public string Text { get; set; }
    public bool IsMemberOnly { get; set; } = false;
    public bool IsSpoiler { get; set; } = false;

    public ApplicationUser? Creator { get; set; }
    public Label? Label { get; set; }

    public LockedPost? LockedPost { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();

    public NpgsqlTsVector SearchVectorOne { get; set; }
    public NpgsqlTsVector SearchVectorTwo { get; set; }

    public Post()
    {
    }

    public Post(PostId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, string friendlyId,
        UserId? creatorId, LabelId? labelId, string title, string titleNormalized, string text, bool isMemberOnly,
        bool isSpoiler) : base(id, groupId, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        LabelId = labelId;
        Title = title;
        TitleNormalized = titleNormalized;
        Text = text;
        IsMemberOnly = isMemberOnly;
        IsSpoiler = isSpoiler;
    }
}