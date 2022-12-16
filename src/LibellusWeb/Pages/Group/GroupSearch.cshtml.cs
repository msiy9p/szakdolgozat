using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Groups.SearchGroupsPaginated;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group;

public class GroupSearchModel : LoggedPageModel<GroupSearchModel>
{
    private readonly ISender _sender;

    public GroupSearchModel(ILogger<GroupSearchModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Group> Groups { get; set; } = new();

    public int FoundCount { get; set; }

    public SortOrder SortOrder { get; set; }

    public async Task<IActionResult> OnGetAsync(string term, int size, int location, string? order)
    {
        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var searchTerm = SearchTerm.Create(term);
        if (searchTerm.IsError)
        {
            return NotFound();
        }

        var sortResult = SortOrderExtensions.FromString(order);
        SortOrder = sortResult.IsError ? SortOrder.Ascending : sortResult.Value;

        var query = new SearchGroupsPaginatedQuery(searchTerm.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var groups = queryResult.Value;
        var url = Url.Page(
            "/Group/GroupSearch",
            pageHandler: null,
            values: new { term = term, size = "25", location = "1" },
            protocol: Request.Scheme);

        FoundCount = groups.TotalCount;
        PageNavigations = PageNavigation.CreateNavigations(groups!, url!);
        Groups.AddRange(groups!.PaginatedItem);

        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string term, string? order)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return new StatusCodeResult(204);
        }

        return RedirectToPage("/Group/GroupSearch", new { term = term, order = order });
    }
}