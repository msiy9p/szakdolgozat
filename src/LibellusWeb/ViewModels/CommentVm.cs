using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class CommentVm
{
    public Comment Comment { get; init; }

    public string GroupId { get; init; }

    public string PostId { get; init; }

    public string LinkBase { get; init; }

    public bool ShowEdit { get; init; }

    public CommentVm(Comment comment, string groupId, string postId, string linkBase, bool showEdit)
    {
        Comment = comment;
        GroupId = groupId;
        PostId = postId;
        LinkBase = linkBase;
        ShowEdit = showEdit;
    }
}