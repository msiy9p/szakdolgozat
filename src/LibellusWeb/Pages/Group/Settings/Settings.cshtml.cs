#pragma warning disable CS8618
using Libellus.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MediatR;
using LibellusWeb.Common;
using Libellus.Application.Commands.Groups.DeleteGroupById;
using Libellus.Application.Commands.Groups.UpdateGroup;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Utilities;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Groups.GetGroupById;
using Libellus.Domain.Common.Interfaces.Services;

namespace LibellusWeb.Pages.Group.Settings;

public class SettingsModel : LoggedPageModel<SettingsModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SettingsModel(ILogger<SettingsModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository,
        IDateTimeProvider dateTimeProvider) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GroupId { get; set; }

    public string Name { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Display(Name = "Description")]
        [StringLength(DescriptionText.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Private?")]
        public bool IsPrivate { get; set; }
    }

    private async Task<Result<GroupId>> GroupExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var groupFriendlyIdId = GroupFriendlyId.Convert(friendlyId);
        if (!groupFriendlyIdId.HasValue)
        {
            return DomainErrors.GroupErrors.GroupNotFound.ToErrorResult<GroupId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(groupFriendlyIdId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        var query = new GetGroupByIdQuery(exists.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Name = queryResult.Value.Name;

        Input = new InputModel()
        {
            Description = queryResult.Value.Description?.Value,
            IsPrivate = queryResult.Value.IsPrivate,
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid)
    {
        GroupId = gid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        var query = new GetGroupByIdQuery(exists.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(Input.Description))
        {
            var descResult = DescriptionText.Create(Input.Description);
            if (descResult.IsError)
            {
                foreach (var error in descResult.Errors)
                {
                    ModelState.AddModelError("", error.Message);
                }

                return Page();
            }

            queryResult.Value.ChangeDescription(descResult.Value, _dateTimeProvider);
        }
        else
        {
            queryResult.Value.RemoveDescription(_dateTimeProvider);
        }

        queryResult.Value.ChangeVisibility(Input.IsPrivate, _dateTimeProvider);

        var command = new UpdateGroupCommand(queryResult.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not edit group.");

            return Page();
        }

        return RedirectToPage("/Group/Settings/Settings",
            new
            {
                gid = GroupId,
            });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        var command = new DeleteGroupByIdCommand(exists.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete group.");

            return Page();
        }

        return RedirectToPage("/Welcome");
    }
}