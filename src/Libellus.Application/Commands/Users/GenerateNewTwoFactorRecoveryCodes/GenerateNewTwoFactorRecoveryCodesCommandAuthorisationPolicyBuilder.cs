using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.GenerateNewTwoFactorRecoveryCodes;

public sealed class GenerateNewTwoFactorRecoveryCodesCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<GenerateNewTwoFactorRecoveryCodesCommand>
{
    public override void BuildPolicy(GenerateNewTwoFactorRecoveryCodesCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}