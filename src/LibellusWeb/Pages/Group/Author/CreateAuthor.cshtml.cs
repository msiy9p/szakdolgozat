#pragma warning disable CS8618
using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Authors.CreateAuthor;

namespace LibellusWeb.Pages.Group.Author;

public class CreateAuthorModel : LoggedPageModel<CreateAuthorModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public CreateAuthorModel(ILogger<CreateAuthorModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    [BindProperty]
    [DataType(DataType.Upload)]
    public IFormFile? Upload { get; set; } = null;

    public class InputModel
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(Libellus.Domain.ValueObjects.Name.MaxLength)]
        public string Name { get; set; }
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

        CoverImageMetaDataContainer? metaDataContainer = null;
        if (Upload is not null)
        {
            Result<ImageDataOnly> imageData;
            await using (var stream = Upload.OpenReadStream())
            {
                if (stream is MemoryStream ms)
                {
                    imageData = ImageDataOnly.Create(ms.ToArray());
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        imageData = ImageDataOnly.Create(memoryStream.ToArray());
                    }
                }
            }

            if (imageData.IsError)
            {
                ModelState.AddModelError("", "Invalid file.");

                return Page();
            }

            var imageCommand = new CreateCoverImagesCommand(imageData.Value);
            var imageCommandResult = await _sender.Send(imageCommand);
            if (imageCommandResult.IsError)
            {
                ModelState.AddModelError("", "Could not create cover image.");
                return Page();
            }

            metaDataContainer = imageCommandResult.Value;
        }

        var command = new CreateAuthorCommand(name.Value!, metaDataContainer);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create author.");

            return Page();
        }

        return RedirectToPage("/Group/Author/Author",
            new { gid = GroupId, aid = commandResult.Value.AuthorFriendlyId.Value });
    }
}