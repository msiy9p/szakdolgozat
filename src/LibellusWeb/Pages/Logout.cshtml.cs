#nullable disable

using Libellus.Application.Commands.Users.SignOut;
using LibellusWeb.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class LogoutModel : LoggedPageModel<LogoutModel>
{
    private readonly ISender _sender;

    public LogoutModel(ILogger<LogoutModel> logger, ISender sender) : base(logger)
    {
        _sender = sender;
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await _sender.Send(SignOutCommand.Instance);

        returnUrl = string.Empty;

        return RedirectToPage("/Login");
    }
}