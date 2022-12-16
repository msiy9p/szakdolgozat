#nullable disable

using System.ComponentModel.DataAnnotations;
using Libellus.Application.Commands.Users.GenerateUserEmailConfirmationToken;
using Libellus.Application.Models;
using Libellus.Application.Queries.Users.GetUserIdByEmail;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class ResendEmailConfirmationModel : LoggedPageModel<ResendEmailConfirmationModel>
{
    private readonly ISender _sender;

    public ResendEmailConfirmationModel(ILogger<ResendEmailConfirmationModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [BindProperty] public InputModel Input { get; set; }

    public class InputModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userCommand = new GetUserIdByEmailQuery(Input.Email);
        var userCommandResult = await _sender.Send(userCommand);
        if (userCommandResult.IsError)
        {
            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }

        var callbackUrl = Url.Page(
            "/ConfirmEmail",
            pageHandler: null,
            values: new
            {
                userId = EmailConfirmationCallbackUrlTemplate.UserIdPlaceholder,
                code = EmailConfirmationCallbackUrlTemplate.TokenPlaceholder
            },
            protocol: Request.Scheme);

        var template = new EmailConfirmationCallbackUrlTemplate(callbackUrl);

        var command = new GenerateUserEmailConfirmationTokenCommand(userCommandResult.Value, template);
        await _sender.Send(command);

        ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
        return Page();
    }
}