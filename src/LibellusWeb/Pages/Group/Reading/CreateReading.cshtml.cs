#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.BookEditions.GetBookEditionById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Readings.CreateReading;

namespace LibellusWeb.Pages.Group.Reading;

public class CreateReadingModel : LoggedPageModel<CreateReadingModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public CreateReadingModel(ILogger<CreateReadingModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }
    public string BookEditionId { get; set; }

    public string CoverLinkBase { get; set; }

    public Libellus.Domain.Entities.Reading Reading { get; set; }
    public Libellus.Domain.Entities.BookEdition BookEdition { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required] [Display(Name = "Reread?")] public bool IsReread { get; set; }

        [Display(Name = "Started on")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateOnly? Started { get; set; }

        [Display(Name = "Finished on")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateOnly? Finished { get; set; }
    }

    private async Task<Result<BookEditionId>> BookEditionExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var bookFriendlyIdId = BookEditionFriendlyId.Convert(friendlyId);
        if (!bookFriendlyIdId.HasValue)
        {
            return DomainErrors.BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(bookFriendlyIdId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        var bookEditionExists = await BookEditionExistsAsync(beid);
        if (bookEditionExists.IsError)
        {
            return NotFound();
        }

        var bookEditionQuery = new GetBookEditionByIdQuery(bookEditionExists.Value);
        var bookEditionQueryResult = await _sender.Send(bookEditionQuery);
        if (bookEditionQueryResult.IsError)
        {
            return NotFound();
        }

        BookEdition = bookEditionQueryResult.Value;

        CoverLinkBase = CreateCoverImageUrlBase();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string beid)
    {
        GroupId = gid;
        BookEditionId = beid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var bookEditionExists = await BookEditionExistsAsync(beid);
        if (bookEditionExists.IsError)
        {
            return NotFound();
        }

        var command = new CreateReadingCommand(bookEditionExists.Value,
            false, Input.IsReread, Input.Started, Input.Finished);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create reading.");

            return Page();
        }

        return RedirectToPage("/Group/Reading/Reading",
            new { gid = GroupId, rid = commandResult.Value!.ReadingFriendlyId.Value });
    }
}