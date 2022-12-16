#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.PasswordSignIn;
using Libellus.Domain.Errors;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class LoginModel : LoggedPageModel<LoginModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public LoginModel(ILogger<LoginModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    [BindProperty] public InputModel Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string ReturnUrl { get; set; }

    [TempData] public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        if (User is not null && _claimsPrincipalExtractor.IsSignedIn(User))
        {
            return RedirectToPage("/Welcome");
        }

        returnUrl ??= Url.Content("/Welcome");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("/Welcome");

        if (ModelState.IsValid)
        {
            var command = new PasswordSignInCommand(Input.Email, Input.Password, Input.RememberMe);
            var commandResult = await _sender.Send(command);

            if (commandResult.IsSuccess)
            {
                return LocalRedirect(returnUrl);
            }

            if (commandResult.Errors.Contains(DomainErrors.UserErrors.UserLoginRequiresTwoFactor))
            {
                return RedirectToPage("/LoginWith2Fa",
                    new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (commandResult.Errors.Contains(DomainErrors.UserErrors.UserIsLockedOut))
            {
                return RedirectToPage("/Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        return Page();
    }
}