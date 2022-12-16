#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.RecoveryCodeSignIn;
using Libellus.Application.Queries.Users.IsInTwoFactorAuthenticationPhase;
using Libellus.Domain.Errors;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class LoginWithRecoveryCodeModel : LoggedPageModel<LoginWithRecoveryCodeModel>
{
    private readonly ISender _sender;

    public LoginWithRecoveryCodeModel(ILogger<LoginWithRecoveryCodeModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [BindProperty] public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        var command = IsInTwoFactorAuthenticationPhaseQuery.Instance;
        var commandResult = await _sender.Send(command);

        if (commandResult.IsError || !commandResult.Value)
        {
            RedirectToPage("/Login");
        }

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);
        var command = new RecoveryCodeSignInCommand(recoveryCode);
        var commandResult = await _sender.Send(command);

        if (commandResult.IsSuccess)
        {
            return LocalRedirect(returnUrl);
        }

        if (commandResult.Errors.Contains(DomainErrors.UserErrors.UserIsLockedOut))
        {
            return RedirectToPage("/Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
        return Page();
    }
}