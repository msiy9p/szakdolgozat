using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class AuthorVm
{
    public Author Author { get; init; }
    public string GroupId { get; init; }
    public string CoverLinkBase { get; init; }
    public bool ShowEditAuthor { get; init; }

    public bool ShowBackToAuthor { get; init; }

    public AuthorVm(Author author, string groupId, string coverLinkBase) :
        this(author, groupId, coverLinkBase, showEditAuthor: false, showBackToAuthor: false)
    {
    }

    public AuthorVm(Author author, string groupId, string coverLinkBase, bool showEditAuthor, bool showBackToAuthor)
    {
        Author = author;
        GroupId = groupId;
        CoverLinkBase = coverLinkBase;
        ShowEditAuthor = showEditAuthor;

        if (ShowEditAuthor)
        {
            ShowBackToAuthor = false;
        }
        else
        {
            ShowBackToAuthor = showBackToAuthor;
        }
    }
}