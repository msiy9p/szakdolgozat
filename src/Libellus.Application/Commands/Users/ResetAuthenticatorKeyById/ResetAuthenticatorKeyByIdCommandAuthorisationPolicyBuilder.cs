using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.ResetAuthenticatorKeyById;

public sealed class ResetAuthenticatorKeyByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<ResetAuthenticatorKeyByIdCommand>
{
    public override void BuildPolicy(ResetAuthenticatorKeyByIdCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}