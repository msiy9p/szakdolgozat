#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.GenerateUserPasswordResetToken;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class ForgotPasswordModel : LoggedPageModel<ForgotPasswordModel>
{
    private readonly ISender _sender;
    private readonly IUserReadOnlyRepository _userRepository;

    public ForgotPasswordModel(ILogger<ForgotPasswordModel> logger, ISender sender,
        IUserReadOnlyRepository userRepository) : base(logger)
    {
        _sender = sender;
        _userRepository = userRepository;
    }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var userId = await _userRepository.GetIdByEmailAsync(Input.Email);
            if (userId.IsError)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error.");
                return Page();
            }

            var callbackUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new
                {
                    code = ForgotPasswordCallbackUrlTemplate.TokenPlaceholder,
                },
                protocol: Request.Scheme);
            var template = new ForgotPasswordCallbackUrlTemplate(callbackUrl);

            var command = new GenerateUserPasswordResetTokenCommand(userId.Value, template);
            await _sender.Send(command);

            return RedirectToPage("/ForgotPasswordConfirmation");
        }

        return Page();
    }
}