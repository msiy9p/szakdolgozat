#pragma warning disable CS8618
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Series.GetAllSeriesPaginated;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Series;

public class SeriesesModel : LoggedPageModel<SeriesesModel>
{
    private readonly ISender _sender;

    public SeriesesModel(ILogger<SeriesesModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public string GroupId { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Series> Series { get; set; } = new();

    public string CoverLinkBase { get; set; }

    public async Task<IActionResult> OnGetAsync(string gid, int size, int location)
    {
        GroupId = gid;

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetAllSeriesPaginatedQuery(paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var shelves = queryResult.Value;
        var url = Url.Page(
            "/Group/Series/Series",
            pageHandler: null,
            values: new { gid = gid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(shelves!, url!);
        Series.AddRange(shelves!.PaginatedItem);

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string gid, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return new StatusCodeResult(204);
        }

        return RedirectToPage("/Group/Series/SeriesSearch", new { gid = gid, term = term });
    }
}