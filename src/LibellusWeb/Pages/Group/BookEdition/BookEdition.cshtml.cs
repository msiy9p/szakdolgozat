#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Enums;
using Libellus.Application.Queries.BookEditions.GetBookEditionByIdWithReadings;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.BookEdition;

public class BookEditionModel : LoggedPageModel<BookEditionModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public BookEditionModel(ILogger<BookEditionModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string BookEditionId { get; set; }

    public Libellus.Domain.Entities.BookEdition BookEdition { get; set; }

    public string CoverLinkBase { get; set; }
    public string ProfileLinkBase { get; set; }

    private async Task<Result<BookEditionId>> BookEditionExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var bookFriendlyId = BookEditionFriendlyId.Convert(friendlyId);
        if (!bookFriendlyId.HasValue)
        {
            return DomainErrors.BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(bookFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        var bookEditionId = await BookEditionExistsAsync(beid);
        if (bookEditionId.IsError)
        {
            return NotFound();
        }

        var query = new GetBookEditionByIdWithReadingsQuery(bookEditionId.Value, SortOrder.Descending);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        BookEdition = queryResult.Value;

        CoverLinkBase = CreateCoverImageUrlBase();
        ProfileLinkBase = CreateProfilePictureUrlBase();

        return Page();
    }
}