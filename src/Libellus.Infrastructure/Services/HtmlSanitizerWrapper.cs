using Ganss.Xss;
using IHtmlSanitizer = Libellus.Application.Common.Interfaces.Services.IHtmlSanitizer;

namespace Libellus.Infrastructure.Services;

internal sealed class HtmlSanitizerWrapper : IHtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer = new HtmlSanitizer();

    public HtmlSanitizerWrapper()
    {
    }

    public string Sanitize(string value)
    {
        return _sanitizer.Sanitize(value);
    }
}