using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.DeleteUserById;

public sealed class
    DeleteUserByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteUserByIdCommand>
{
    public override void BuildPolicy(DeleteUserByIdCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}