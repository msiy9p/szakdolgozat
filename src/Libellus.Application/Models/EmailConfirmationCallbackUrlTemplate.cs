using Ardalis.GuardClauses;
using Libellus.Application.Common.Models;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Models;

public sealed class EmailConfirmationCallbackUrlTemplate : BaseCallbackUrlTemplate
{
    public const string UserIdPlaceholder = "__UserId__";
    public const string TokenPlaceholder = "__token__";

    public string Template { get; init; }

    public EmailConfirmationCallbackUrlTemplate(string template)
    {
        Guard.Against.NullOrWhiteSpace(template);

        Template = template;
    }

    public string CreateUrl(UserId userId, string token)
    {
        return Encode(
            Template.Replace(UserIdPlaceholder, userId.ToString())
                .Replace(TokenPlaceholder, token));
    }
}