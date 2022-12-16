#pragma warning disable CS8618
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Shelves.GetAllShelvesPaginated;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Shelf;

public class ShelvesModel : LoggedPageModel<ShelvesModel>
{
    private readonly ISender _sender;

    public ShelvesModel(ILogger<ShelvesModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public string GroupId { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Shelf> Shelves { get; set; } = new();

    public string CoverLinkBase { get; set; }

    public async Task<IActionResult> OnGetAsync(string gid, int size, int location)
    {
        GroupId = gid;

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetAllShelvesPaginatedQuery(paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var shelves = queryResult.Value;
        var url = Url.Page(
            "/Group/Shelf/Shelves",
            pageHandler: null,
            values: new { gid = gid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(shelves!, url!);
        Shelves.AddRange(shelves!.PaginatedItem);

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string gid, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return new StatusCodeResult(204);
        }

        return RedirectToPage("/Group/Shelf/ShelfSearch", new { gid = gid, term = term });
    }
}