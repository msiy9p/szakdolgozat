#pragma warning disable CS8618

using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Shelves.CreateShelf;

namespace LibellusWeb.Pages.Group.Shelf;

public class CreateShelfModel : LoggedPageModel<CreateShelfModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public CreateShelfModel(ILogger<CreateShelfModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(Libellus.Domain.ValueObjects.Name.MaxLength)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(DescriptionText.MaxLength)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;

        [Required] [Display(Name = "Locked?")] public bool IsLocked { get; set; }
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

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid)
    {
        GroupId = gid;

        var exists = await GroupExistsAsync(gid);
        if (exists.IsError)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var name = Name.Create(Input.Name);
        if (name.IsError)
        {
            foreach (var error in name.Errors)
            {
                ModelState.AddModelError("", error.Message);
            }

            return Page();
        }

        DescriptionText? description = null;
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

            description = descResult.Value;
        }

        var command = new CreateShelfCommand(name.Value!, description, Input.IsLocked);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create post.");

            return Page();
        }

        return RedirectToPage("/Group/Shelf/Shelf",
            new { gid = GroupId, shid = commandResult.Value.SeriesFriendlyId.Value });
    }
}