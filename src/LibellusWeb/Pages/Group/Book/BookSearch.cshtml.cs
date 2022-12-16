#pragma warning disable CS8618

using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.Books.SearchBooksPaginated;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Book;

public class BookSearchModel : LoggedPageModel<BookSearchModel>
{
    private readonly ISender _sender;

    public BookSearchModel(ILogger<BookSearchModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public string GroupId { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.Book> Books { get; set; } = new();

    public int FoundCount { get; set; }

    public string CoverLinkBase { get; set; }

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

        var query = new SearchBooksPaginatedQuery(searchTerm.Value, paginationInfo.Value.PageNumber,
            (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        var books = queryResult.Value;
        var url = Url.Page(
            "/Group/Book/BookSearch",
            pageHandler: null,
            values: new { gid = gid, term = term, size = "25", location = "1" },
            protocol: Request.Scheme);

        FoundCount = books.TotalCount;
        PageNavigations = PageNavigation.CreateNavigations(books!, url!);
        Books.AddRange(books!.PaginatedItem);
        InputSearchTerm = searchTerm.Value.Value;

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