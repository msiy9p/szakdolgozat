#nullable disable

using Libellus.Application.Commands.Users.GenerateNewTwoFactorRecoveryCodes;
using Libellus.Application.Queries.Users.IsTwoFactorEnabledById;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class GenerateRecoveryCodesModel : LoggedPageModel<GenerateRecoveryCodesModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public GenerateRecoveryCodesModel(ILogger<GenerateRecoveryCodesModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    [TempData] public string[] RecoveryCodes { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var is2FaEnabledQuery = new IsTwoFactorEnabledByIdQuery(result.Value);
        var is2FaEnabledQueryResult = await _sender.Send(is2FaEnabledQuery);
        if (is2FaEnabledQueryResult.IsError)
        {
            throw new InvalidOperationException(
                $"Cannot generate recovery codes for user because they do not have 2FA enabled.");
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

        var is2FaEnabledQuery = new IsTwoFactorEnabledByIdQuery(result.Value);
        var is2FaEnabledQueryResult = await _sender.Send(is2FaEnabledQuery);
        if (is2FaEnabledQueryResult.IsError)
        {
            throw new InvalidOperationException(
                $"Cannot generate recovery codes for user because they do not have 2FA enabled.");
        }

        var generateCommand = new GenerateNewTwoFactorRecoveryCodesCommand(result.Value);
        var generateCommandResult = await _sender.Send(generateCommand);
        if (generateCommandResult.IsError)
        {
            ModelState.AddModelError(string.Empty, "Could not generate new recovery code.");
            return Page();
        }

        RecoveryCodes = generateCommandResult.Value!.ToArray();

        StatusMessage = "You have generated new recovery codes.";
        return RedirectToPage("./ShowRecoveryCodes");
    }
}