#nullable disable

using LibellusWeb.Common;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages;

public class RegisterConfirmationModel : LoggedPageModel<RegisterConfirmationModel>
{
    public RegisterConfirmationModel(ILogger<RegisterConfirmationModel> logger) : base(logger)
    {
    }

    public string Email { get; set; }

    public IActionResult OnGet(string email, string returnUrl = null)
    {
        if (email == null)
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }
}