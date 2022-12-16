#nullable disable

using LibellusWeb.Common;

namespace LibellusWeb.Pages;

public class LockoutModel : LoggedPageModel<LockoutModel>
{
    public LockoutModel(ILogger<LockoutModel> logger) : base(logger)
    {
    }

    public void OnGet()
    {
    }
}