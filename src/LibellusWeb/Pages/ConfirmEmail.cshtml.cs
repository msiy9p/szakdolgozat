#nullable disable

using Libellus.Application.Commands.Users.ConfirmUserEmail;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class ConfirmEmailModel : LoggedPageModel<ConfirmEmailModel>
{
    private readonly ISender _sender;

    public ConfirmEmailModel(ILogger<ConfirmEmailModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [TempData] public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string code)
    {
        if (userId == null || code == null || !Guid.TryParse(userId, out var guidId))
        {
            return RedirectToPage("/Login");
        }

        var command = new ConfirmUserEmailCommand(new UserId(guidId), code);
        var commandResult = await _sender.Send(command);

        if (commandResult.IsError)
        {
            if (commandResult.Errors.Contains(DomainErrors.UserErrors.UserNotFound))
            {
                return NotFound("Unable to load user.");
            }

            StatusMessage = "Error confirming your email.";
            return Page();
        }

        StatusMessage = "Thank you for confirming your email.";
        return Page();
    }
}