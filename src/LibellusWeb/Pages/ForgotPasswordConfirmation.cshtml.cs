#nullable disable

using LibellusWeb.Common;

namespace LibellusWeb.Pages;

public class ForgotPasswordConfirmationModel : LoggedPageModel<ForgotPasswordConfirmationModel>
{
    public ForgotPasswordConfirmationModel(ILogger<ForgotPasswordConfirmationModel> logger) : base(logger)
    {
    }

    public void OnGet()
    {
    }
}