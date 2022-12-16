#nullable disable
using Libellus.Application.Commands.ProfilePictures.CreateProfilePictures;
using Libellus.Application.Commands.Users.ChangeUserProfilePicture;
using Libellus.Application.Commands.Users.DeleteUserProfilePicture;
using Libellus.Application.Queries.Users.GetUserPicturedVmById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class IndexModel : LoggedPageModel<IndexModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public IndexModel(ILogger<IndexModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public string Username { get; set; }

    public string LinkBase { get; set; }

    public ProfilePictureMetaDataContainer? AvailableProfilePictures { get; set; }

    [BindProperty] public IFormFile Upload { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var command = new GetUserPicturedVmByIdQuery(result.Value);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            return NotFound("Unable to load user.");
        }

        Username = commandResult.Value.UserName;
        AvailableProfilePictures = commandResult.Value.AvailableProfilePictures;

        var profurl = Url.Page(
            "/Images/ProfilePicture",
            pageHandler: null,
            values: new { id = "x" },
            protocol: Request.Scheme);

        LinkBase = profurl.Substring(0, profurl.Length - 2);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Upload is null)
        {
            ModelState.AddModelError("", "Invalid file.");

            return Page();
        }

        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
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

        var imageCommand = new CreateProfilePicturesCommand(imageData.Value);
        var imageCommandResult = await _sender.Send(imageCommand);
        if (imageCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create profile picture.");

            return Page();
        }

        var changeCommand = new ChangeUserProfilePictureCommand(result.Value, imageCommandResult.Value.Id);
        var changeCommandResult = await _sender.Send(changeCommand);
        if (changeCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not create profile picture.");

            return Page();
        }

        return RedirectToPage("/Account/Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var deleteCommand = new DeleteUserProfilePictureCommand(result.Value);
        var deleteCommandResult = await _sender.Send(deleteCommand);
        if (deleteCommandResult.IsError)
        {
            ModelState.AddModelError("", "Could not delete profile picture.");

            return Page();
        }

        return RedirectToPage("/Account/Index");
    }
}