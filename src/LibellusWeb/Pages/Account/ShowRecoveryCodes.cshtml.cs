#nullable disable

using LibellusWeb.Common;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Account;

public class ShowRecoveryCodesModel : LoggedPageModel<ShowRecoveryCodesModel>
{
    public ShowRecoveryCodesModel(ILogger<ShowRecoveryCodesModel> logger) : base(logger)
    {
    }

    [TempData] public string[] RecoveryCodes { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        if (RecoveryCodes == null || RecoveryCodes.Length == 0)
        {
            return RedirectToPage("./TwoFactorAuthentication");
        }

        return Page();
    }
}