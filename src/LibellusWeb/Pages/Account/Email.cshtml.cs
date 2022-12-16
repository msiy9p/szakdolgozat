#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.GenerateChangeUserEmailToken;
using Libellus.Application.Models;
using Libellus.Application.Queries.Users.GetEmailById;
using Libellus.Domain.Common.Types.Ids;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class EmailModel : LoggedPageModel<EmailModel>
{
    private readonly ISender _sender;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public EmailModel(ILogger<EmailModel> logger, ISender sender,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) :
        base(logger)
    {
        _sender = sender;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public string Email { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }

    private async Task LoadAsync(UserId userId)
    {
        var emailCommand = new GetEmailByIdQuery(userId);
        var emailCommandResult = await _sender.Send(emailCommand);

        if (emailCommandResult.IsError)
        {
            RedirectToPage(NotFound("Unable to load user."));
        }

        Email = emailCommandResult.Value;

        Input = new InputModel
        {
            NewEmail = emailCommandResult.Value,
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        await LoadAsync(result.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(result.Value);
            return Page();
        }

        var emailCommand = new GetEmailByIdQuery(result.Value);
        var emailCommandResult = await _sender.Send(emailCommand);

        if (emailCommandResult.IsError)
        {
            return NotFound("Unable to load user.");
        }

        if (Input.NewEmail != emailCommandResult.Value)
        {
            var userResult = _claimsPrincipalExtractor.GetUserId(User);
            if (result.IsError)
            {
                return NotFound("Unable to load user.");
            }

            var callbackUrl = Url.Page(
                "/ConfirmEmailChange",
                pageHandler: null,
                values: new
                {
                    userId = EmailConfirmationCallbackUrlTemplate.UserIdPlaceholder,
                    email = Input.NewEmail,
                    code = EmailConfirmationCallbackUrlTemplate.TokenPlaceholder
                },
                protocol: Request.Scheme);

            var template = new EmailConfirmationCallbackUrlTemplate(callbackUrl);

            var changeCommand =
                new GenerateChangeUserEmailTokenCommand(userResult.Value, Input.NewEmail, template);
            var changeCommandResult = await _sender.Send(changeCommand);
            if (changeCommandResult.IsError)
            {
                foreach (var error in changeCommandResult.Errors)
                {
                    ModelState.AddModelError("", error.Message);
                }

                return Page();
            }

            StatusMessage = "Confirmation link to change email sent. Please check your email.";
            return RedirectToPage();
        }

        StatusMessage = "Your email is unchanged.";
        return RedirectToPage();
    }
}