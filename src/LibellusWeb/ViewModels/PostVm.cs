using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class PostVm
{
    public Post Post { get; init; }

    public string GroupId { get; init; }

    public string PostId { get; init; }

    public string LinkBase { get; init; }

    public bool ShowCreateComment { get; init; }

    public bool ShowEditPost { get; init; }

    public bool ShowBackToPost { get; init; }

    public PostVm(Post post, string groupId, string postId, string linkBase) : this(post, groupId, postId,
        linkBase, showCreateComment: false, showEditPost: false, showBackToPost: false)
    {
    }

    public PostVm(Post post, string groupId, string postId, string linkBase,
        bool showCreateComment, bool showEditPost, bool showBackToPost)
    {
        Post = post;
        GroupId = groupId;
        PostId = postId;
        LinkBase = linkBase;
        ShowCreateComment = showCreateComment;
        ShowEditPost = showEditPost;

        if (ShowEditPost)
        {
            ShowBackToPost = false;
        }
        else
        {
            ShowBackToPost = showBackToPost;
        }
    }
}