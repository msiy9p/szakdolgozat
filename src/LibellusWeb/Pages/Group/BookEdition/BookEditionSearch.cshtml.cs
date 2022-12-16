#pragma warning disable CS8618

using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.Queries.BookEditions.GetBookEditionByIsbnPaginated;
using Libellus.Application.Queries.BookEditions.SearchBookEditionsPaginated;
using Libellus.Domain.Common.Types;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using LibellusWeb.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.BookEdition;

public class BookEditionSearchModel : LoggedPageModel<BookEditionSearchModel>
{
    private readonly ISender _sender;

    public BookEditionSearchModel(ILogger<BookEditionSearchModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public string GroupId { get; set; }

    public List<PageNavigation> PageNavigations { get; set; } = new();

    public List<Libellus.Domain.Entities.BookEdition> BookEditions { get; set; } = new();

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

        Isbn? isbn = null;
        if (searchTerm.Value.ValueNormalized.Length >= 14 && searchTerm.Value.ValueNormalized.Substring(0, 4) == "ISBN")
        {
            var split = searchTerm.Value.Value.Split(':', 2, StringSplitOptions.TrimEntries);
            isbn = Isbn.Convert(split[1]);
        }

        if (isbn.HasValue)
        {
            var query = new GetBookEditionByIsbnPaginatedQuery(isbn.Value, paginationInfo.Value.PageNumber,
                (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
            var queryResult = await _sender.Send(query);
            if (queryResult.IsError)
            {
                return NotFound();
            }

            var books = queryResult.Value;
            var url = Url.Page(
                "/Group/BookEdition/BookEditionSearch",
                pageHandler: null,
                values: new { gid = gid, term = term, size = "25", location = "1" },
                protocol: Request.Scheme);

            FoundCount = books.TotalCount;
            PageNavigations = PageNavigation.CreateNavigations(books!, url!);
            BookEditions.AddRange(books!.PaginatedItem);
        }
        else
        {
            var query = new SearchBookEditionsPaginatedQuery(searchTerm.Value, paginationInfo.Value.PageNumber,
                (int)paginationInfo.Value.ItemCount, SortOrder.Ascending);
            var queryResult = await _sender.Send(query);
            if (queryResult.IsError)
            {
                return NotFound();
            }

            var books = queryResult.Value;
            var url = Url.Page(
                "/Group/BookEdition/BookEditionSearch",
                pageHandler: null,
                values: new { gid = gid, term = term, size = "25", location = "1" },
                protocol: Request.Scheme);

            FoundCount = books.TotalCount;
            PageNavigations = PageNavigation.CreateNavigations(books!, url!);
            BookEditions.AddRange(books!.PaginatedItem);
        }


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

        return RedirectToPage("/Group/BookEdition/BookEditionSearch", new { gid = gid, term = term });
    }
}