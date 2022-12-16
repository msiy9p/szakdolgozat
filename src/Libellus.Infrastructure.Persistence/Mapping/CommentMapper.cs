using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using DomainComment = Libellus.Domain.Entities.Comment;
using PersistenceComment = Libellus.Infrastructure.Persistence.DataModels.Comment;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct CommentMapper : IMapFrom<PersistenceComment, UserPicturedVm, Result<DomainComment>>,
    IMapFrom<DomainComment, PersistenceComment>, IMapFrom<DomainComment, PostId, PersistenceComment>,
    IMapFrom<DomainComment, GroupId, PersistenceComment>, IMapFrom<DomainComment, GroupId, PostId, PersistenceComment>
{
    public static Result<DomainComment> Map(PersistenceComment item1, UserPicturedVm item2)
    {
        return DomainComment.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new CommentFriendlyId(item1.FriendlyId),
            item2,
            (CommentText)item1.Text,
            item1.RepliedToId);
    }

    public static PersistenceComment Map(DomainComment item1)
    {
        return new PersistenceComment()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            Text = item1.Text,
            RepliedToId = item1.RepliedTo,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc
        };
    }

    public static PersistenceComment Map(DomainComment item1, PostId item2)
    {
        var comment = Map(item1);
        comment.PostId = item2;

        return comment;
    }

    public static PersistenceComment Map(DomainComment item1, GroupId item2)
    {
        var comment = Map(item1);
        comment.GroupId = item2;

        return comment;
    }

    public static PersistenceComment Map(DomainComment item1, GroupId item2, PostId item3)
    {
        var comment = Map(item1, item2);
        comment.PostId = item3;

        return comment;
    }
}