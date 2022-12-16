#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.CreateUser;
using Libellus.Application.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class RegisterModel : LoggedPageModel<RegisterModel>
{
    private readonly ISender _sender;

    public RegisterModel(ILogger<RegisterModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [BindProperty] public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 1)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var callbackUrl = Url.Page(
                "/ConfirmEmail",
                pageHandler: null,
                values: new
                {
                    userId = EmailConfirmationCallbackUrlTemplate.UserIdPlaceholder,
                    code = EmailConfirmationCallbackUrlTemplate.TokenPlaceholder,
                    returnUrl = returnUrl
                },
                protocol: Request.Scheme);

            var template = new EmailConfirmationCallbackUrlTemplate(callbackUrl);

            var command = new CreateUserCommand(Input.Email, Input.UserName, Input.Password, CreateWithDefaults: true,
                template);
            var commandResult = await _sender.Send(command);
            if (commandResult.IsError)
            {
                foreach (var error in commandResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
            }
            else
            {
                return RedirectToPage("/RegisterConfirmation",
                    new { email = Input.Email, returnUrl = returnUrl });
            }
        }

        return Page();
    }
}