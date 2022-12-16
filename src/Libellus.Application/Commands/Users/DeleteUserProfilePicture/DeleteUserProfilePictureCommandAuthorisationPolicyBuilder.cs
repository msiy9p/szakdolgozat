using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Users.DeleteUserProfilePicture;

public sealed class DeleteUserProfilePictureCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<DeleteUserProfilePictureCommand>
{
    public override void BuildPolicy(DeleteUserProfilePictureCommand instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}