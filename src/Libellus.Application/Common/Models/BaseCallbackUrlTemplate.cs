using System.Text.Encodings.Web;

namespace Libellus.Application.Common.Models;

public abstract class BaseCallbackUrlTemplate
{
    protected static string Encode(string value)
    {
        return HtmlEncoder.Default.Encode(value).Replace("&amp;", "&");
    }
}