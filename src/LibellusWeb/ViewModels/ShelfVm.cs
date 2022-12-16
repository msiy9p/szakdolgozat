using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class ShelfVm
{
    public Shelf Shelf { get; init; }

    public string GroupId { get; init; }

    public string ShelfId { get; init; }

    public bool ShowEditShelf { get; init; }

    public bool ShowBackToShelf { get; init; }

    public ShelfVm(Shelf shelf, string groupId, string shelfId) : this(shelf, groupId, shelfId,
        showEditShelf: false, showBackToShelf: false)
    {
    }

    public ShelfVm(Shelf shelf, string groupId, string shelfId, bool showEditShelf,
        bool showBackToShelf)
    {
        Shelf = shelf;
        GroupId = groupId;
        ShelfId = shelfId;
        ShowEditShelf = showEditShelf;

        if (ShowEditShelf)
        {
            ShowBackToShelf = false;
        }
        else
        {
            ShowBackToShelf = showBackToShelf;
        }
    }
}