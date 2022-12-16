using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.EnableTwoFactorById;

public sealed class
    EnableTwoFactorByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        EnableTwoFactorByIdCommand>
{
    public override void BuildPolicy(EnableTwoFactorByIdCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}