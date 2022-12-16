using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibellusWeb.Common;

public abstract class LoggedPageModel<TPage> : PageModel where TPage : PageModel
{
    protected readonly ILogger<TPage> _logger;

    private string _profilePictureUrlBase = string.Empty;
    private string _coverImageUrlBase = string.Empty;

    protected LoggedPageModel(ILogger<TPage> logger)
    {
        _logger = logger;
    }

    protected string CreateProfilePictureUrlBase()
    {
        if (!string.IsNullOrWhiteSpace(_profilePictureUrlBase))
        {
            return _profilePictureUrlBase;
        }

        var profurl = Url.Page(
            "/Images/ProfilePicture",
            pageHandler: null,
            values: new { id = "x" },
            protocol: Request.Scheme);

        if (string.IsNullOrEmpty(profurl))
        {
            return string.Empty;
        }

        _profilePictureUrlBase = profurl.Substring(0, profurl.Length - 2);

        return _profilePictureUrlBase;
    }

    protected string CreateCoverImageUrlBase()
    {
        if (!string.IsNullOrWhiteSpace(_coverImageUrlBase))
        {
            return _coverImageUrlBase;
        }

        var coverurl = Url.Page(
            "/Images/CoverImage",
            pageHandler: null,
            values: new { id = "x" },
            protocol: Request.Scheme);

        if (string.IsNullOrEmpty(coverurl))
        {
            return string.Empty;
        }

        _coverImageUrlBase = coverurl.Substring(0, coverurl.Length - 2);

        return _coverImageUrlBase;
    }
}