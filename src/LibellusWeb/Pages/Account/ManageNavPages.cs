#nullable disable

using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibellusWeb.Pages.Account;

public static class ManageNavPages
{
    public const string Index = "Index";

    public const string Email = "Email";

    public const string ChangePassword = "ChangePassword";

    public const string DownloadPersonalData = "DownloadPersonalData";

    public const string DeletePersonalData = "DeletePersonalData";

    public const string PersonalData = "PersonalData";

    public const string TwoFactorAuthentication = "TwoFactorAuthentication";

    public const string Invitations = "Invitations";

    public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

    public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);

    public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

    public static string DownloadPersonalDataNavClass(ViewContext viewContext) =>
        PageNavClass(viewContext, DownloadPersonalData);

    public static string DeletePersonalDataNavClass(ViewContext viewContext) =>
        PageNavClass(viewContext, DeletePersonalData);

    public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

    public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) =>
        PageNavClass(viewContext, TwoFactorAuthentication);

    public static string InvitationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Invitations);

    public static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }
}