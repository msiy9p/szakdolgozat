using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class BookVm
{
    public Book Book { get; init; }
    public string GroupId { get; init; }
    public string CoverLinkBase { get; init; }

    public BookVm(Book book, string groupId, string coverLinkBase)
    {
        Book = book;
        GroupId = groupId;
        CoverLinkBase = coverLinkBase;
    }
}