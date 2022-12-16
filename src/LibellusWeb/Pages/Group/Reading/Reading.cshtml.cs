#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.BookEditions.GetBookEditionById;
using Libellus.Application.Queries.Readings.GetReadingById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Reading;

public class ReadingModel : LoggedPageModel<ReadingModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public ReadingModel(ILogger<ReadingModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }
    public string ReadingId { get; set; }

    public string CoverLinkBase { get; set; }
    public string ProfileLinkBase { get; set; }

    public Libellus.Domain.Entities.Reading Reading { get; set; }
    public Libellus.Domain.Entities.BookEdition BookEdition { get; set; }

    private async Task<Result<ReadingId>> ReadingExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var readingFriendlyId = ReadingFriendlyId.Convert(friendlyId);
        if (!readingFriendlyId.HasValue)
        {
            return DomainErrors.ReadingErrors.ReadingNotFound.ToErrorResult<ReadingId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(readingFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string rid)
    {
        GroupId = gid;
        ReadingId = rid;

        var readingId = await ReadingExistsAsync(rid);
        if (readingId.IsError)
        {
            return NotFound();
        }

        var readingQuery = new GetReadingByIdQuery(readingId.Value);
        var readingQueryResult = await _sender.Send(readingQuery);
        if (readingQueryResult.IsError)
        {
            return NotFound();
        }

        Reading = readingQueryResult.Value;

        var bookEditionQuery = new GetBookEditionByIdQuery(Reading.BookEditionId);
        var bookEditionQueryResult = await _sender.Send(bookEditionQuery);
        if (bookEditionQueryResult.IsError)
        {
            return NotFound();
        }

        BookEdition = bookEditionQueryResult.Value;

        CoverLinkBase = CreateCoverImageUrlBase();
        ProfileLinkBase = CreateProfilePictureUrlBase();

        return Page();
    }
}