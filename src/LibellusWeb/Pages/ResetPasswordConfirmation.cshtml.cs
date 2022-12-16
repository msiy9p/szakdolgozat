#nullable disable

using LibellusWeb.Common;

namespace LibellusWeb.Pages;

public class ResetPasswordConfirmationModel : LoggedPageModel<ResetPasswordConfirmationModel>
{
    public ResetPasswordConfirmationModel(ILogger<ResetPasswordConfirmationModel> logger) : base(logger)
    {
    }

    public void OnGet()
    {
    }
}