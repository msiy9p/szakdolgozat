using Ardalis.GuardClauses;
using Libellus.Application.Common.Models;

namespace Libellus.Application.Models;

public sealed class ForgotPasswordCallbackUrlTemplate : BaseCallbackUrlTemplate
{
    public const string TokenPlaceholder = "__token__";

    public string Template { get; init; }

    public ForgotPasswordCallbackUrlTemplate(string template)
    {
        Guard.Against.NullOrWhiteSpace(template);

        Template = template;
    }

    public string CreateUrl(string token)
    {
        return Encode(
            Template.Replace(TokenPlaceholder, token));
    }
}