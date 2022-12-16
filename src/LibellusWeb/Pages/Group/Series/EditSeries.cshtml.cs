#pragma warning disable CS8618
using Libellus.Application.Commands.Series.DeleteSeriesById;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Series.GetSeriesById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Series.UpdateSeriesById;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;

namespace LibellusWeb.Pages.Group.Series;

public class EditSeriesModel : LoggedPageModel<EditSeriesModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public EditSeriesModel(ILogger<EditSeriesModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string SeriesId { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(Libellus.Domain.ValueObjects.Title.MaxLength)]
        [DataType(DataType.Text)]
        public string Title { get; set; } = string.Empty;
    }

    private async Task<Result<SeriesId>> SeriesExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var seriesFriendlyId = SeriesFriendlyId.Convert(friendlyId);
        if (!seriesFriendlyId.HasValue)
        {
            return DomainErrors.SeriesErrors.SeriesNotFound.ToErrorResult<SeriesId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(seriesFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string seid)
    {
        GroupId = gid;
        SeriesId = seid;

        var seriesId = await SeriesExistsAsync(seid);
        if (seriesId.IsError)
        {
            return NotFound();
        }

        var query = new GetSeriesByIdQuery(seriesId.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Input = new InputModel()
        {
            Title = queryResult.Value.Title,
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string seid)
    {
        GroupId = gid;
        SeriesId = seid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var seriesId = await SeriesExistsAsync(seid);
        if (seriesId.IsError)
        {
            return NotFound();
        }

        var title = Title.Create(Input.Title);
        if (title.IsError)
        {
            foreach (var error in title.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        var command = new UpdateSeriesByIdCommand(seriesId.Value, title);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit series.");

            return Page();
        }

        return RedirectToPage("/Group/Series/Series", new { gid = GroupId, seid = SeriesId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string seid)
    {
        GroupId = gid;
        SeriesId = seid;

        var seriesId = await SeriesExistsAsync(seid);
        if (seriesId.IsError)
        {
            return NotFound();
        }

        var command = new DeleteSeriesByIdCommand(seriesId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete series.");

            return Page();
        }

        return RedirectToPage("/Group/Series/Series", new { gid = GroupId });
    }
}