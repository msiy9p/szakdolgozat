#pragma warning disable CS8618
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Posts.GetAllPostsPaginated;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post;

public class PostsModel : LoggedPageModel<PostsModel>
{
    private readonly ISender _sender;

    public PostsModel(ILogger<PostsModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Post> Posts { get; set; } = new();

    public string GroupId { get; set; }

    public string LinkBase { get; set; }

    public async Task<IActionResult> OnGetAsync(string gid, int size, int location, string? labelName, string? order)
    {
        GroupId = gid;

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetAllPostsPaginatedQuery(paginationInfo.Value.PageNumber,
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
            values: new { gid = gid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(posts!, url!);
        Posts.AddRange(posts!.PaginatedItem);

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