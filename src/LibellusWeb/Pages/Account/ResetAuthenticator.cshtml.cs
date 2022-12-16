#nullable disable

using Libellus.Application.Commands.Users.ResetAuthenticatorKeyById;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class ResetAuthenticatorModel : LoggedPageModel<ResetAuthenticatorModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public ResetAuthenticatorModel(ILogger<ResetAuthenticatorModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    [TempData] public string StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
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

        var resetCommand = new ResetAuthenticatorKeyByIdCommand(result.Value);
        var resetCommandResult = await _sender.Send(resetCommand);

        if (resetCommandResult.IsError)
        {
            return NotFound($"Unable to load user.");
        }

        StatusMessage =
            "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

        return RedirectToPage("./EnableAuthenticator");
    }
}