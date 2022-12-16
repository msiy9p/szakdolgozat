#nullable disable

using Libellus.Application.Commands.Users.DisableTwoFactorById;
using Libellus.Application.Queries.Users.IsTwoFactorEnabledById;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class Disable2FaModel : LoggedPageModel<Disable2FaModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public Disable2FaModel(ILogger<Disable2FaModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    [TempData] public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var twoFaQuery = new IsTwoFactorEnabledByIdQuery(result.Value);
        var twoFaQueryResult = await _sender.Send(twoFaQuery);

        if (twoFaQueryResult.IsError || !twoFaQueryResult.Value)
        {
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var disable2faCommand = new DisableTwoFactorByIdCommand(result.Value);
        var disable2faCommandResult = await _sender.Send(disable2faCommand);
        if (disable2faCommandResult.IsError)
        {
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
        }

        StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app.";
        return RedirectToPage("./TwoFactorAuthentication");
    }
}