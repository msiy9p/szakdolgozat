namespace Libellus.Application.Common.Interfaces.Services;

public interface IHtmlSanitizer
{
    string Sanitize(string value);
}