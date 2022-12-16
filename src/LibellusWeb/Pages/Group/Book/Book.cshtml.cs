#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Queries.Books.GetBookByIdWithBookEdition;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Book;

public class BookModel : LoggedPageModel<BookModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public BookModel(ILogger<BookModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string BookId { get; set; }

    public Libellus.Domain.Entities.Book Book { get; set; }

    public string CoverLinkBase { get; set; }

    private async Task<Result<BookId>> BookExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var bookFriendlyId = BookFriendlyId.Convert(friendlyId);
        if (!bookFriendlyId.HasValue)
        {
            return DomainErrors.BookErrors.BookNotFound.ToErrorResult<BookId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(bookFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string bid)
    {
        GroupId = gid;
        BookId = bid;

        var bookId = await BookExistsAsync(bid);
        if (bookId.IsError)
        {
            return NotFound();
        }

        var query = new GetBookByIdWithBookEditionQuery(bookId.Value, SortOrder.Ascending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Book = queryResult.Value;

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }
}