using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using DomainComment = Libellus.Domain.Entities.Comment;
using DomainLabel = Libellus.Domain.Entities.Label;
using DomainPost = Libellus.Domain.Entities.Post;
using PersistencePost = Libellus.Infrastructure.Persistence.DataModels.Post;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct PostMapper : IMapFrom<PersistencePost, UserPicturedVm?, Result<DomainPost>>,
    IMapFrom<PersistencePost, UserPicturedVm?, ICollection<DomainComment>, Result<DomainPost>>,
    IMapFrom<DomainPost, PersistencePost>,
    IMapFrom<DomainPost, GroupId, PersistencePost>
{
    public static Result<DomainPost> Map(PersistencePost item1, UserPicturedVm? item2)
    {
        var label = item1.Label is null ? null : LabelMapper.Map(item1.Label!).Value;

        return DomainPost.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new PostFriendlyId(item1.FriendlyId),
            item2,
            label,
            (Title)item1.Title,
            (CommentText)item1.Text,
            item1.IsMemberOnly,
            item1.IsSpoiler,
            item1.LockedPost is not null);
    }

    public static Result<DomainPost> Map(PersistencePost item1, UserPicturedVm? item2, ICollection<DomainComment> item3)
    {
        var label = item1.Label is null ? null : LabelMapper.Map(item1.Label!).Value;

        return DomainPost.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new PostFriendlyId(item1.FriendlyId),
            item2,
            label,
            (Title)item1.Title,
            (CommentText)item1.Text,
            item3,
            item1.IsMemberOnly,
            item1.IsSpoiler,
            item1.LockedPost is not null);
    }

    public static PersistencePost Map(DomainPost item1)
    {
        return new PersistencePost()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            LabelId = item1.Label?.Id,
            Title = item1.Title.Value,
            TitleNormalized = item1.Title.ValueNormalized,
            Text = item1.Text,
            IsMemberOnly = item1.IsMemberOnly,
            IsSpoiler = item1.IsSpoiler,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc
        };
    }

    public static PersistencePost Map(DomainPost item1, GroupId item2)
    {
        var post = Map(item1);
        post.GroupId = item2;

        return post;
    }
}