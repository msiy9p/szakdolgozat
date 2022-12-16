#pragma warning disable CS8618
using Libellus.Application.Queries.ProfilePictures.GetProfilePictureByObjectName;
using Libellus.Domain.Enums;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace LibellusWeb.Pages.Images;

public class ProfilePictureModel : LoggedPageModel<ProfilePictureModel>
{
    private const int CacheDurationInSeconds = 60 * 60 * 24;

    private readonly ISender _sender;

    public ProfilePictureModel(ILogger<ProfilePictureModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public string Id { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Id = id;

        if (string.IsNullOrWhiteSpace(Id))
        {
            return NotFound();
        }

        var command = new GetProfilePictureByObjectNameQuery(Id);
        var commandResult = await _sender.Send(command);
        if (commandResult.IsError)
        {
            return NotFound();
        }

        var mime = ImageFormatExtensions.ToMimeType(commandResult.Value!.ImageFormat);
        if (mime.IsError)
        {
            return NotFound();
        }

        Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + CacheDurationInSeconds;

        return File(commandResult.Value!.Data, mime.Value!, commandResult.Value!.ToString());
    }
}