using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.ChangeUserProfilePicture;

public sealed class ChangeUserProfilePictureCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<ChangeUserProfilePictureCommand>
{
    public override void BuildPolicy(ChangeUserProfilePictureCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}