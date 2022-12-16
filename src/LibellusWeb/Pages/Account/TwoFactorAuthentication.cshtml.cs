#nullable disable

using Libellus.Application.Commands.Users.ForgetTwoFactorClient;
using Libellus.Application.Queries.Users.GetTwoFactorSummaryById;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class TwoFactorAuthenticationModel : LoggedPageModel<TwoFactorAuthenticationModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public TwoFactorAuthenticationModel(ILogger<TwoFactorAuthenticationModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    [BindProperty] public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var summaryQuery = new GetTwoFactorSummaryByIdQuery(result.Value);
        var summaryQueryResult = await _sender.Send(summaryQuery);
        if (summaryQueryResult.IsError)
        {
            return NotFound($"Unable to load user.");
        }

        HasAuthenticator = summaryQueryResult.Value!.HasAuthenticator;
        Is2faEnabled = summaryQueryResult.Value!.Is2FaEnabled;
        IsMachineRemembered = summaryQueryResult.Value!.IsMachineRemembered;
        RecoveryCodesLeft = summaryQueryResult.Value!.RecoveryCodesLeft;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var forgetCommand = ForgetTwoFactorClientCommand.Instance;
        var forgetCommandResult = await _sender.Send(forgetCommand);
        if (forgetCommandResult.IsError)
        {
            return NotFound($"Unable to load user.");
        }

        StatusMessage =
            "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return RedirectToPage();
    }
}