using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.DisableTwoFactorById;

public sealed class
    DisableTwoFactorByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DisableTwoFactorByIdCommand>
{
    public override void BuildPolicy(DisableTwoFactorByIdCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}