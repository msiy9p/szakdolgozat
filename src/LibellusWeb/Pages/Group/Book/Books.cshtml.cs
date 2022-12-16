#pragma warning disable CS8618

using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Books.GetAllBooksPaginated;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Book;

public class BooksModel : LoggedPageModel<BooksModel>
{
    private readonly ISender _sender;

    public BooksModel(ILogger<BooksModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public string GroupId { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Book> Books { get; set; } = new();

    public string CoverLinkBase { get; set; }

    public async Task<IActionResult> OnGetAsync(string gid, int size, int location)
    {
        GroupId = gid;

        var paginationInfo = PaginationInfo.Create(location, size, adjustItemCount: true);
        if (paginationInfo.IsError)
        {
            return NotFound();
        }

        var query = new GetAllBooksPaginatedQuery(paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var books = queryResult.Value;
        var url = Url.Page(
            "/Group/Book/Books",
            pageHandler: null,
            values: new { gid = gid, size = "25", location = "1" },
            protocol: Request.Scheme);

        PageNavigations = PageNavigation.CreateNavigations(books!, url!);
        Books.AddRange(books!.PaginatedItem);

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync(string gid, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return new StatusCodeResult(204);
        }

        return RedirectToPage("/Group/Book/BookSearch", new { gid = gid, term = term });
    }
}