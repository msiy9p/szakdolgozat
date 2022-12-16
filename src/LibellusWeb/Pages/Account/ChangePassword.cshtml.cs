#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.ChangeUserPassword;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class ChangePasswordModel : LoggedPageModel<ChangePasswordModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public ChangePasswordModel(ILogger<ChangePasswordModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    [BindProperty] public InputModel Input { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        var passwordCommand = new ChangeUserPasswordCommand(result.Value, Input.OldPassword, Input.NewPassword);
        var passwordCommandResult = await _sender.Send(passwordCommand);

        if (passwordCommandResult.IsError)
        {
            foreach (var error in passwordCommandResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return Page();
        }

        StatusMessage = "Your password has been changed.";

        return RedirectToPage();
    }
}