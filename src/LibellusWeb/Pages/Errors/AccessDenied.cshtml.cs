using LibellusWeb.Common;

namespace LibellusWeb.Pages.Errors;

public sealed class AccessDeniedModel : LoggedPageModel<AccessDeniedModel>
{
    public AccessDeniedModel(ILogger<AccessDeniedModel> logger) : base(logger)
    {
    }

    public void OnGet()
    {
    }
}