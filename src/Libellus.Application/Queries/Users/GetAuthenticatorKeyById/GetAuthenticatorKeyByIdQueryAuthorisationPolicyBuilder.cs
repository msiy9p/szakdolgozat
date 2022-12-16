using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Users.GetAuthenticatorKeyById;

public sealed class
    GetAuthenticatorKeyByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAuthenticatorKeyByIdQuery>
{
    public override void BuildPolicy(GetAuthenticatorKeyByIdQuery instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}