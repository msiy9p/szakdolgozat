using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.ChangeUserPassword;

public sealed class
    ChangeUserPasswordCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        ChangeUserPasswordCommand>
{
    public override void BuildPolicy(ChangeUserPasswordCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}