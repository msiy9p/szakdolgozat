#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using Libellus.Application.Commands.Users.EnableTwoFactorById;
using Libellus.Application.Queries.Users.GetAuthenticatorKeyById;
using Libellus.Application.Queries.Users.GetEmailById;
using Libellus.Application.Queries.Users.IsTwoFactorTokenValid;
using Libellus.Domain.Common.Types.Ids;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class EnableAuthenticatorModel : LoggedPageModel<EnableAuthenticatorModel>
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private readonly ISender _sender;
    private readonly UrlEncoder _urlEncoder;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public EnableAuthenticatorModel(ILogger<EnableAuthenticatorModel> logger, ISender sender, UrlEncoder urlEncoder,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) :
        base(logger)
    {
        _sender = sender;
        _urlEncoder = urlEncoder;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public string SharedKey { get; set; }

    public string AuthenticatorUri { get; set; }

    [TempData] public string[] RecoveryCodes { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        await LoadSharedKeyAndQrCodeUriAsync(result.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(result.Value);
            return Page();
        }

        // Strip spaces and hyphens
        var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var verifyQuery = new IsTwoFactorTokenValidQuery(result.Value, verificationCode);
        var verifyQueryResult = await _sender.Send(verifyQuery);

        if (verifyQueryResult.IsError || !verifyQueryResult.Value)
        {
            ModelState.AddModelError("Input.Code", "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(result.Value);
            return Page();
        }

        var enableCommand = new EnableTwoFactorByIdCommand(result.Value);
        var enableCommandResult = await _sender.Send(enableCommand);
        if (enableCommandResult.IsError)
        {
            return NotFound($"Unable to load user.");
        }

        StatusMessage = "Your authenticator app has been verified.";

        RecoveryCodes = enableCommandResult.Value!.ToArray();
        return RedirectToPage("./ShowRecoveryCodes");
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(UserId userId)
    {
        // Load the authenticator key & QR code URI to display on the form
        var keyQuery = new GetAuthenticatorKeyByIdQuery(userId);
        var keyQueryResult = await _sender.Send(keyQuery);
        if (keyQueryResult.IsError)
        {
            RedirectToPage(NotFound("Unable to load user."));
        }

        SharedKey = FormatKey(keyQueryResult.Value);

        var emailCommand = new GetEmailByIdQuery(userId);
        var emailCommandResult = await _sender.Send(emailCommand);

        if (emailCommandResult.IsError)
        {
            RedirectToPage(NotFound("Unable to load user."));
        }

        AuthenticatorUri = GenerateQrCodeUri(emailCommandResult.Value, keyQueryResult.Value);
    }

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            _urlEncoder.Encode(nameof(Libellus)),
            _urlEncoder.Encode(email),
            unformattedKey);
    }
}