#nullable disable

using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibellusWeb.Pages.Group;

public static class ManageNavPages
{
    public static readonly string Authors = nameof(Authors);

    public static readonly string Books = nameof(Books);

    public static readonly string BookEditions = nameof(BookEditions);

    public static readonly string Posts = nameof(Posts);

    public static readonly string Readings = nameof(Readings);

    public static readonly string Series = nameof(Series);

    public static readonly string Shelves = nameof(Shelves);

    public static readonly string Settings = nameof(Settings);

    public static string AuthorsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Authors);

    public static string BooksNavClass(ViewContext viewContext) => PageNavClass(viewContext, Books);

    public static string BookEditionsNavClass(ViewContext viewContext) => PageNavClass(viewContext, BookEditions);

    public static string PostsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Posts);

    public static string ReadingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Readings);

    public static string SeriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Series);

    public static string ShelvesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Shelves);

    public static string SettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Settings);

    public static string PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }
}