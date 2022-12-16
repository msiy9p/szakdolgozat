#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.TwoFactorSignIn;
using Libellus.Application.Queries.Users.IsInTwoFactorAuthenticationPhase;
using Libellus.Domain.Errors;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class LoginWith2FaModel : LoggedPageModel<LoginWith2FaModel>
{
    private readonly ISender _sender;

    public LoginWith2FaModel(ILogger<LoginWith2FaModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [BindProperty] public InputModel Input { get; set; }

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
    {
        var command = IsInTwoFactorAuthenticationPhaseQuery.Instance;
        var commandResult = await _sender.Send(command);

        if (commandResult.IsError || !commandResult.Value)
        {
            RedirectToPage("/Login");
        }

        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        returnUrl ??= Url.Content("/Welcome");

        var command = new TwoFactorSignInCommand(Input.TwoFactorCode, rememberMe, Input.RememberMachine);
        var commandResult = await _sender.Send(command);

        if (commandResult.IsSuccess)
        {
            return LocalRedirect(returnUrl);
        }

        if (commandResult.Errors.Contains(DomainErrors.UserErrors.UserIsLockedOut))
        {
            return RedirectToPage("/Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
        return Page();
    }
}