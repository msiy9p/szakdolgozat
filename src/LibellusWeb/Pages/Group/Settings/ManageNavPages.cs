#nullable disable

using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibellusWeb.Pages.Group.Settings;

public static class ManageNavPages
{
    public static readonly string Settings = nameof(Settings);

    public static readonly string Members = nameof(Members);

    public static readonly string Invitations = nameof(Invitations);

    public static readonly string Tags = nameof(Tags);


    public static string SettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Settings);

    public static string MembersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Members);

    public static string InvitationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Invitations);

    public static string TagsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Tags);

    public static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }
}