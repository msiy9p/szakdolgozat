#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.BookEditions.GetBookEditionById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Libellus.Application.Queries.Readings.GetReadingById;
using Libellus.Domain.Utilities;
using Libellus.Application.Commands.Readings.UpdateReadingById;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Commands.Readings.DeleteReadingById;

namespace LibellusWeb.Pages.Group.Reading;

public class EditReadingModel : LoggedPageModel<EditReadingModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public EditReadingModel(ILogger<EditReadingModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository,
        IHtmlSanitizer htmlSanitizer) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _htmlSanitizer = htmlSanitizer;
    }

    public string GroupId { get; set; }
    public string ReadingId { get; set; }

    public string CoverLinkBase { get; set; }

    public Libellus.Domain.Entities.Reading Reading { get; set; }
    public Libellus.Domain.Entities.BookEdition BookEdition { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required] [Display(Name = "Reread?")] public bool IsReread { get; set; }

        [Required] [Display(Name = "Is DNF?")] public bool IsDnf { get; set; }

        [Display(Name = "Started on")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateOnly? Started { get; set; }

        [Display(Name = "Finished on")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateOnly? Finished { get; set; }

        [Display(Name = "Notes")]
        [StringLength(CommentText.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.Html)]
        public string? Text { get; set; } = string.Empty;
    }

    private async Task<Result<ReadingId>> ReadingExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var readingFriendlyId = ReadingFriendlyId.Convert(friendlyId);
        if (!readingFriendlyId.HasValue)
        {
            return DomainErrors.BookEditionErrors.BookEditionNotFound.ToErrorResult<ReadingId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(readingFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string rid)
    {
        GroupId = gid;
        ReadingId = rid;

        var readingExists = await ReadingExistsAsync(rid);
        if (readingExists.IsError)
        {
            return NotFound();
        }

        var readingQuery = new GetReadingByIdQuery(readingExists.Value);
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

        var startedDt = Reading.StartedOnUtc?.ToDateTimeUtc();
        var finishedDt = Reading.FinishedOnUtc?.ToDateTimeUtc();

        DateOnly? started = null;
        if (startedDt.HasValue)
        {
            started = new DateOnly(startedDt.Value.Year, startedDt.Value.Month, startedDt.Value.Day);
        }

        DateOnly? finished = null;
        if (finishedDt.HasValue)
        {
            finished = new DateOnly(finishedDt.Value.Year, finishedDt.Value.Month, finishedDt.Value.Day);
        }

        Input = new InputModel()
        {
            IsDnf = Reading.IsDnf,
            IsReread = Reading.IsReread,
            Text = Reading.Note?.Text.Value,
            Started = started,
            Finished = finished,
        };

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string rid)
    {
        GroupId = gid;
        ReadingId = rid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var readingExists = await ReadingExistsAsync(rid);
        if (readingExists.IsError)
        {
            return NotFound();
        }

        CommentText? comment = null;
        if (!string.IsNullOrWhiteSpace(Input.Text))
        {
            var text = CommentText.Create(_htmlSanitizer.Sanitize(WebUtility.HtmlDecode(Input.Text)));
            if (text.IsError)
            {
                foreach (var error in text.Errors)
                {
                    ModelState.AddModelError("", error.Message);
                }

                return Page();
            }

            comment = text.Value;
        }

        var command = new UpdateReadingByIdCommand(readingExists.Value,
            Input.IsDnf, Input.IsReread, Input.Started, Input.Finished, comment);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit reading.");

            return Page();
        }

        return RedirectToPage("/Group/Reading/Reading",
            new { gid = GroupId, rid = ReadingId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string rid)
    {
        GroupId = gid;
        ReadingId = rid;

        var readingExists = await ReadingExistsAsync(rid);
        if (readingExists.IsError)
        {
            return NotFound();
        }

        var command = new DeleteReadingByIdCommand(readingExists.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete reading.");

            return Page();
        }

        return RedirectToPage("/Group/Reading/Readings",
            new { gid = GroupId });
    }
}