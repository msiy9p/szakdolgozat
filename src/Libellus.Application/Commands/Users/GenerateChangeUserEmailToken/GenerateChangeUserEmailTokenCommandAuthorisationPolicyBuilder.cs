using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.GenerateChangeUserEmailToken;

public sealed class
    GenerateChangeUserEmailTokenCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GenerateChangeUserEmailTokenCommand>
{
    public override void BuildPolicy(GenerateChangeUserEmailTokenCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}