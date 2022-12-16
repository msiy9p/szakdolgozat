#nullable disable

using Libellus.Application.Commands.Users.ChangeUserEmail;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class ConfirmEmailChangeModel : LoggedPageModel<ConfirmEmailChangeModel>
{
    private readonly ISender _sender;

    public ConfirmEmailChangeModel(ILogger<ConfirmEmailChangeModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    [TempData] public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
    {
        if (userId == null || email == null || code == null || !Guid.TryParse(userId, out var guidId))
        {
            return RedirectToPage("/Login");
        }

        var command = new ChangeUserEmailCommand(new UserId(guidId), email, code);
        var commandResult = await _sender.Send(command);

        if (commandResult.IsError)
        {
            if (commandResult.Errors.Contains(DomainErrors.UserErrors.UserNotFound))
            {
                return NotFound("Unable to load user.");
            }

            StatusMessage = "Error changing email.";
            return Page();
        }

        StatusMessage = "Thank you for confirming your email change.";
        return Page();
    }
}