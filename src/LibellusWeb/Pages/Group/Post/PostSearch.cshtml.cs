#pragma warning disable CS8618
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Posts.SearchPostsPaginated;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post;

public class PostSearchModel : LoggedPageModel<PostSearchModel>
{
    private readonly ISender _sender;

    public PostSearchModel(ILogger<PostSearchModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Post> Posts { get; set; } = new();

    public string GroupId { get; set; }

    public string LinkBase { get; set; }

    public int FoundCount { get; set; }

    public string InputSearchTerm { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string gid, string term, int size, int location)
    {
        GroupId = gid;

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

        var query = new SearchPostsPaginatedQuery(searchTerm.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Descending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var posts = queryResult.Value;
        var url = Url.Page(
            "/Group/Post/Posts",
            pageHandler: null,
            values: new { gid = gid, size = "25", location = "1", term = term },
            protocol: Request.Scheme);

        FoundCount = posts.TotalCount;
        PageNavigations = PageNavigation.CreateNavigations(posts!, url!);
        Posts.AddRange(posts!.PaginatedItem);
        InputSearchTerm = searchTerm.Value.Value;

        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string gid, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return new StatusCodeResult(204);
        }

        return RedirectToPage("/Group/Post/PostSearch", new { gid = gid, term = term });
    }
}