#pragma warning disable CS8618
using Libellus.Application.Commands.CoverImages.CreateCoverImages;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Authors.GetAuthorById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Authors.DeleteAuthorCoverImageById;
using Libellus.Application.Commands.Authors.UpdateAuthorById;
using Libellus.Application.Commands.Authors.UpdateAuthorCoverImageById;
using Libellus.Domain.ValueObjects;
using Libellus.Application.Commands.Authors.DeleteAuthorById;

namespace LibellusWeb.Pages.Group.Author;

public class EditAuthorModel : LoggedPageModel<EditAuthorModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public EditAuthorModel(ILogger<EditAuthorModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    public string GroupId { get; set; }

    public string AuthorId { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty]
    [DataType(DataType.Upload)]
    public IFormFile? Upload { get; set; } = null;

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Display(Name = "Name")]
        [StringLength(Libellus.Domain.ValueObjects.Name.MaxLength)]
        [DataType(DataType.Text)]
        public string Name { get; set; } = string.Empty;
    }

    private async Task<Result<AuthorId>> AuthorExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var authorFriendlyId = AuthorFriendlyId.Convert(friendlyId);
        if (!authorFriendlyId.HasValue)
        {
            return DomainErrors.AuthorErrors.AuthorNotFound.ToErrorResult<AuthorId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(authorFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string aid)
    {
        GroupId = gid;
        AuthorId = aid;

        var authorId = await AuthorExistsAsync(aid);
        if (authorId.IsError)
        {
            return NotFound();
        }

        var query = new GetAuthorByIdQuery(authorId.Value);
        var queryResult = await _sender.Send(query);
        if (queryResult.IsError)
        {
            return NotFound();
        }

        Input = new InputModel()
        {
            Name = queryResult.Value.Name.Value
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string gid, string aid)
    {
        GroupId = gid;
        AuthorId = aid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var authorId = await AuthorExistsAsync(aid);
        if (authorId.IsError)
        {
            return NotFound();
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

        var updateCommand = new UpdateAuthorByIdCommand(authorId.Value, name.Value);
        var updateCommandResult = await _sender.Send(updateCommand);
        if (updateCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not update author.");

            return Page();
        }

        return RedirectToPage("/Group/Author/Author", new { gid = GroupId, aid = AuthorId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string gid, string aid)
    {
        GroupId = gid;
        AuthorId = aid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var authorId = await AuthorExistsAsync(aid);
        if (authorId.IsError)
        {
            return NotFound();
        }

        var command = new DeleteAuthorByIdCommand(authorId.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete author.");

            return Page();
        }

        return RedirectToPage("/Group/Author/Authors", new { gid = GroupId });
    }

    public async Task<IActionResult> OnPostCoverChangeAsync(string gid, string aid)
    {
        GroupId = gid;
        AuthorId = aid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Upload is null)
        {
            ModelState.AddModelError("", "Invalid file.");

            return Page();
        }

        var authorId = await AuthorExistsAsync(aid);
        if (authorId.IsError)
        {
            return NotFound();
        }

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

        var command = new UpdateAuthorCoverImageByIdCommand(authorId.Value, imageCommandResult.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            ModelState.AddModelError("", "Could not change cover image.");

            return Page();
        }

        return RedirectToPage("/Group/Author/Author", new { gid = GroupId, aid = AuthorId });
    }

    public async Task<IActionResult> OnPostCoverDeleteAsync(string gid, string aid)
    {
        GroupId = gid;
        AuthorId = aid;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var authorId = await AuthorExistsAsync(aid);
        if (authorId.IsError)
        {
            return NotFound();
        }

        var deleteCommand = new DeleteAuthorCoverImageByIdCommand(authorId.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete cover image.");

            return Page();
        }

        return RedirectToPage("/Group/Author/Author", new { gid = GroupId, aid = AuthorId });
    }
}