using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Groups.GetAllGroupsPaginated;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group;

public class GroupsModel : LoggedPageModel<GroupsModel>
{
    private readonly ISender _sender;

    public GroupsModel(ILogger<GroupsModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Group> Groups { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int size, int location, string? order)
    {
        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var sortResult = SortOrderExtensions.FromString(order);
        var sort = sortResult.IsError ? SortOrder.Ascending : sortResult.Value;

        var query = new GetAllGroupsPaginatedQuery(paginationInfo.Value.PageNumber, (int)paginationInfo.Value.ItemCount,
            sort);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var groups = queryResult.Value;
        var url = Url.Page(
            "/Group/Groups",
            pageHandler: null,
            values: new { size = "25", location = "1" },
            protocol: Request.Scheme);

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