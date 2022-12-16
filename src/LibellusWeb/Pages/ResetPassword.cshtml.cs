#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using Libellus.Application.Commands.Users.ResetUserPasswordByEmail;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace LibellusWeb.Pages;

public class ResetPasswordModel : LoggedPageModel<ResetPasswordModel>
{
    private readonly ISender _sender;

    public ResetPasswordModel(ILogger<ResetPasswordModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required] public string Code { get; set; }
    }

    public IActionResult OnGet(string code = null)
    {
        if (code == null)
        {
            return BadRequest("A code must be supplied for password reset.");
        }

        Input = new InputModel
        {
            Code = code
            //Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var command = new ResetUserPasswordByEmailCommand(Input.Email, Input.Code, Input.Password);
        await _sender.Send(command);

        return RedirectToPage("/ResetPasswordConfirmation");
    }
}