using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class BookEditionVm
{
    public BookEdition BookEdition { get; init; }
    public string GroupId { get; init; }
    public string CoverLinkBase { get; init; }
    public bool ShowLink { get; init; }

    public BookEditionVm(BookEdition bookEdition, string groupId, string coverLinkBase, bool showLink)
    {
        BookEdition = bookEdition;
        GroupId = groupId;
        CoverLinkBase = coverLinkBase;
        ShowLink = showLink;
    }
}